using System.Diagnostics;
using Microsoft.Data.SqlClient;
using MQTTnet;
using MQTTnet.Protocol;

namespace TESCWatchDog;

public partial class Form1
{
    private CancellationTokenSource? monitorCancellation;
    private int databaseQueryTimeoutSeconds = 10;
    private DateTime? lastDatabaseTimestamp;
    private DateTimeOffset lastDatabaseReceivedAt = DateTimeOffset.Now;
    private long lastNoDataSeconds;
    // 佇列由 UI 執行緒持有；MQTT 接收回呼不直接操作佇列。
    private readonly Queue<(string Topic, string Payload)> pendingNotifications = new();

    private void InitializeDatabaseSettings()
    {
        clientIdTextBox.Text = $"TESCWatchDog-{Guid.NewGuid():N}";
        var defaultIni = Path.Combine(AppContext.BaseDirectory, "system.ini");
        if (File.Exists(defaultIni)) LoadCdbIni(defaultIni);
    }

    private void IntegratedAuth_CheckedChanged(object? sender, EventArgs e)
    {
        dbUser.Enabled = dbPassword.Enabled = !integratedAuth.Checked;
    }

    private void StopMonitor_Click(object? sender, EventArgs e)
    {
        stopMonitor.Enabled = false;
        monitorCancellation?.Cancel();
    }

    private void LoadCdbIni(string path)
    {
        try
        {
            // Parse fully before changing any UI fields; never log file contents or passwords.
            var settings = CdbIniSettings.Parse(File.ReadAllText(path, System.Text.Encoding.UTF8));
            dbServer.Text = settings.Server; dbName.Text = settings.Database;
            dbUser.Text = settings.User; dbPassword.Text = settings.Password;
            dbSchema.Text = settings.Schema;
            integratedAuth.Checked = settings.IntegratedSecurity;
            dbUser.Enabled = dbPassword.Enabled = !settings.IntegratedSecurity;
            trustDbCertificate.Checked = settings.TrustServerCertificate;
            databaseQueryTimeoutSeconds = settings.QueryTimeoutSeconds;
            dbStatus.Text = "已載入 CDB 設定，尚未連線";
            AppendLog("已載入 CDB INI 設定；尚未開始監控。");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or FormatException)
        {
            string message = ex is FormatException ? ex.Message : "無法讀取設定檔，請確認路徑與讀取權限。";
            AppendLog($"載入 INI 失敗：{message}");
        }
    }

    private static void ValidateNotification(string topic, string payload)
    {
        if (string.IsNullOrWhiteSpace(topic) || topic.IndexOfAny(['+', '#', '\0']) >= 0 || System.Text.Encoding.UTF8.GetByteCount(topic) > 65535)
            throw new InvalidOperationException("通知 Topic 不可空白、含萬用字元或超過 MQTT 長度限制。");
        if (string.IsNullOrWhiteSpace(payload)) throw new InvalidOperationException("通知內容不可空白。");
    }

    private async void StartMonitor_Click(object? sender, EventArgs e)
    {
        if (monitorCancellation != null) return;
        try
        {
            if (string.IsNullOrWhiteSpace(dbServer.Text) || string.IsNullOrWhiteSpace(dbName.Text))
                throw new InvalidOperationException("請填寫 SQL Server 與資料庫名稱。");
            if (!integratedAuth.Checked && string.IsNullOrWhiteSpace(dbUser.Text))
                throw new InvalidOperationException("請填寫 SQL 帳號，或選擇 Windows 驗證。");
            if (string.IsNullOrWhiteSpace(dbSchema.Text) || dbSchema.Text.Length > 128 || dbSchema.Text.Contains('\0'))
                throw new InvalidOperationException("請填寫有效的 Schema 名稱。");
            if (autoAlertCheckBox.Checked) ValidateNotification(alertTopicTextBox.Text.Trim(), alertPayloadTextBox.Text);
            if (recoveryEnabled.Checked) ValidateNotification(recoveryTopic.Text.Trim(), recoveryPayload.Text);
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = dbServer.Text.Trim(), InitialCatalog = dbName.Text.Trim(),
                IntegratedSecurity = integratedAuth.Checked, ConnectTimeout = Math.Min(10, databaseQueryTimeoutSeconds),
                ConnectRetryCount = 0,
                Encrypt = SqlConnectionEncryptOption.Mandatory, TrustServerCertificate = trustDbCertificate.Checked,
                ApplicationName = "TESCWatchDog", PersistSecurityInfo = false
            };
            if (!integratedAuth.Checked) { builder.UserID = dbUser.Text; builder.Password = dbPassword.Text; }
            using var cancellation = new CancellationTokenSource();
            monitorCancellation = cancellation;
            startMonitor.Enabled = false; stopMonitor.Enabled = true; SetMonitorConfigurationEnabled(false);
            AppendLog("CDB 監控開始：唯讀 VM_RESULT_CONTROL；第一次成功查詢建立基準。");
            await MonitorDatabaseAsync(builder.ConnectionString, cancellation.Token);
        }
        catch (OperationCanceledException) { AppendLog("已停止 CDB 監控。"); }
        catch (Exception ex) { AppendLog($"監控停止：{ex.Message}"); }
        finally
        {
            monitorCancellation = null;
            if (!IsDisposed && !Disposing)
            {
                startMonitor.Enabled = true; stopMonitor.Enabled = false; SetMonitorConfigurationEnabled(true);
                dbStatus.Text = $"已停止；待送通知 {pendingNotifications.Count} 筆（重啟監控後重試）";
            }
        }
    }

    private async Task MonitorDatabaseAsync(string connectionString, CancellationToken token)
    {
        // 啟動時擷取設定；每次查詢完整 await，避免重疊 SQL 或累積計時器工作。
        var interval = TimeSpan.FromSeconds((double)pollSeconds.Value);
        var queryLimit = TimeSpan.FromSeconds(databaseQueryTimeoutSeconds);
        var quotedSchema = "[" + dbSchema.Text.Replace("]", "]]") + "]";
        var state = new DatabaseWatchState();
        var clock = Stopwatch.StartNew();
        var lastObserved = DateTimeOffset.Now;
        var queryFailed = false;
        while (true)
        {
            token.ThrowIfCancellationRequested();
            var pollStarted = clock.Elapsed;
            dbStatus.Text = $"查詢中；待送通知 {pendingNotifications.Count} 筆";
            AppendLog($"正在查詢 CDB：{quotedSchema}.[VM_RESULT_CONTROL] 最新 TIMETAG...");
            DateTime? latest = null;
            bool success = false;
            // 此逾時涵蓋開啟連線與 SQL 執行；停止監控的 token 優先處理。
            using var queryCancellation = CancellationTokenSource.CreateLinkedTokenSource(token);
            queryCancellation.CancelAfter(queryLimit);
            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync(queryCancellation.Token);
                await using var command = connection.CreateCommand();
                command.CommandText = $"SELECT MAX([TIMETAG]) FROM {quotedSchema}.[VM_RESULT_CONTROL];";
                command.CommandTimeout = (int)queryLimit.TotalSeconds;
                var value = await command.ExecuteScalarAsync(queryCancellation.Token);
                latest = value is null or DBNull ? null : (DateTime)value;
                success = true;
                if (queryFailed) AppendLog("CDB 查詢已恢復。");
                queryFailed = false;
            }
            catch (Exception ex) when (!token.IsCancellationRequested)
            {
                bool timedOut = queryCancellation.IsCancellationRequested || ex is SqlException { Number: -2 };
                AppendLog(timedOut
                    ? $"CDB 查詢逾時（上限 {queryLimit.TotalSeconds:F0} 秒）；本次不判定資料警報，等待 {interval.TotalSeconds:F0} 秒後重試。"
                    : $"CDB 查詢異常（不判定為無資料）：{ex.Message}；等待 {interval.TotalSeconds:F0} 秒後重試。");
                queryFailed = true;
                dbStatus.Text = "CDB 查詢失敗，等待下次重試";
            }
            token.ThrowIfCancellationRequested();
            if (success)
            {
                bool baseline = !state.Initialized;
                bool changed = latest.HasValue && (!state.Latest.HasValue || latest > state.Latest);
                var querySeconds = (clock.Elapsed - pollStarted).TotalSeconds;
                var elapsed = baseline ? TimeSpan.Zero : clock.Elapsed - state.LastChange;
                string? transition = state.Observe(latest, clock.Elapsed, TimeSpan.FromSeconds((double)timeoutSecondsNumeric.Value));
                if (baseline || changed) lastObserved = DateTimeOffset.Now;
                lastDatabaseTimestamp = state.Latest;
                lastDatabaseReceivedAt = lastObserved;
                lastNoDataSeconds = (long)(clock.Elapsed - state.LastChange).TotalSeconds;
                if (baseline) AppendLog($"CDB 基準已建立：{latest?.ToString("O") ?? "空表／TIMETAG 全為 NULL"}；現在開始計時。");
                AppendLog($"CDB 查詢完成（耗時 {querySeconds:F1} 秒）：{(baseline ? "首次基準" : changed ? "發現新資料" : "沒有新資料")}；最新 TIMETAG：{latest?.ToString("O") ?? "無"}；累計無新資料 {(clock.Elapsed - state.LastChange).TotalSeconds:F0} 秒。");
                if (transition != null)
                {
                    AppendLog(transition == "alert" ? "CDB 已達無新資料門檻。" : "CDB 已出現新資料，恢復正常。");
                    bool enabled = transition == "alert" ? autoAlertCheckBox.Checked : recoveryEnabled.Checked;
                    if (enabled)
                    {
                        if (pendingNotifications.Count >= 100) throw new InvalidOperationException("待送通知已達 100 筆，請恢復 MQTT 連線後重新開始監控。");
                        var topic = transition == "alert" ? alertTopicTextBox.Text.Trim() : recoveryTopic.Text.Trim();
                        var template = transition == "alert" ? alertPayloadTextBox.Text : recoveryPayload.Text;
                        var payload = template.Replace("{timestamp}", DateTimeOffset.Now.ToString("O"))
                            .Replace("{seconds}", ((long)elapsed.TotalSeconds).ToString(System.Globalization.CultureInfo.InvariantCulture))
                            .Replace("{lastData}", state.Latest?.ToString("O") ?? "")
                            .Replace("{lastReceived}", lastObserved.ToString("O"));
                        pendingNotifications.Enqueue((topic, payload));
                    }
                }
                dbStatus.Text = $"{(state.InAlarm ? "無新資料" : "監控中")}：{(clock.Elapsed - state.LastChange).TotalSeconds:F0} 秒；待送 {pendingNotifications.Count}";
            }
            // 單次僅處理一筆待送，避免 Broker 恢復時大量通知長時間阻塞查詢。
            await FlushNotificationsAsync(token);
            var elapsedWork = clock.Elapsed - pollStarted;
            var delay = PollTiming.NextDelay(interval, elapsedWork, success);
            if (elapsedWork >= interval)
                AppendLog($"本輪工作耗時 {elapsedWork.TotalSeconds:F1} 秒；完成後仍倒數 {delay.TotalSeconds:F0} 秒再查，不補跑。");
            await DelayWithQueryCountdownAsync(delay, dbStatus.Text, token);
        }
    }

    private void SetMonitorConfigurationEnabled(bool enabled)
    {
        cdbGroup.Enabled = enabled;
        autoAlertCheckBox.Enabled = enabled;
        alertTopicTextBox.Enabled = enabled;
        alertPayloadTextBox.Enabled = enabled;
        recoveryEnabled.Enabled = enabled;
        recoveryTopic.Enabled = enabled;
        recoveryPayload.Enabled = enabled;
    }

    private async void PushAlertButton_Click(object? sender, EventArgs e) =>
        await PushManualNotificationAsync(pushAlertButton, "警報", alertTopicTextBox.Text, alertPayloadTextBox.Text);

    private async void PushRecoveryButton_Click(object? sender, EventArgs e) =>
        await PushManualNotificationAsync(pushRecoveryButton, "恢復", recoveryTopic.Text, recoveryPayload.Text);

    private async Task PushManualNotificationAsync(Button button, string kind, string topicText, string template)
    {
        if (!mqttClient.IsConnected)
        {
            AppendLog($"無法手動推送{kind}：MQTT 尚未連線。");
            return;
        }

        try
        {
            var topic = topicText.Trim();
            ValidateNotification(topic, template);
            button.Enabled = false;
            var payload = template.Replace("{timestamp}", DateTimeOffset.Now.ToString("O"))
                .Replace("{seconds}", lastNoDataSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture))
                .Replace("{lastData}", lastDatabaseTimestamp?.ToString("O") ?? "")
                .Replace("{lastReceived}", lastDatabaseReceivedAt.ToString("O"));
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            var result = await PublishWithReceiptAsync(new MqttApplicationMessageBuilder()
                .WithTopic(topic).WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag(false).Build(), timeout.Token);
            if ((int)result.ReasonCode >= 128) throw new InvalidOperationException($"Broker 拒絕：{result.ReasonCode}");
            AppendLog($"已手動推送{kind} [{topic}] QoS 1／不保留。");
        }
        catch (Exception ex)
        {
            AppendLog($"手動推送{kind}失敗：{ex.Message}");
        }
        finally
        {
            if (!IsDisposed && !Disposing) button.Enabled = mqttClient.IsConnected;
        }
    }

    /// <summary>
    /// 等候下一輪查詢時，每秒更新狀態列，讓使用者可直接看到剩餘等待時間。
    /// 此倒數只顯示排程等待時間，不包含目前這一輪 SQL 查詢所花的時間。
    /// </summary>
    private async Task DelayWithQueryCountdownAsync(TimeSpan delay, string statusText, CancellationToken token)
    {
        var remaining = delay;
        while (remaining > TimeSpan.Zero)
        {
            var seconds = (int)Math.Ceiling(remaining.TotalSeconds);
            dbStatus.Text = $"{statusText}；距離下次查詢 {seconds} 秒";
            AppendLog($"距離下次查詢 CDB：{seconds} 秒");
            var step = remaining > TimeSpan.FromSeconds(1) ? TimeSpan.FromSeconds(1) : remaining;
            await Task.Delay(step, token);
            remaining -= step;
        }
    }

    private async Task FlushNotificationsAsync(CancellationToken token)
    {
        if (pendingNotifications.TryPeek(out var notification) && mqttClient.IsConnected)
        {
            try
            {
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(token);
                // 訂閱驗證與發布各有時間預算，訂閱失敗不能耗光發布時間。
                timeout.CancelAfter(TimeSpan.FromSeconds(20));
                var result = await PublishWithReceiptAsync(new MqttApplicationMessageBuilder()
                    .WithTopic(notification.Topic).WithPayload(notification.Payload)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).WithRetainFlag(false).Build(), timeout.Token);
                if ((int)result.ReasonCode >= 128) throw new InvalidOperationException($"Broker 拒絕：{result.ReasonCode}");
                pendingNotifications.Dequeue();
                AppendLog($"通知已發布 [{notification.Topic}] QoS 1／不保留。");
            }
            catch (Exception ex) when (!token.IsCancellationRequested)
            {
                AppendLog($"通知發布失敗，保留待重試：{ex.Message}");
                return;
            }
        }
    }
}
