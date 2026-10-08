using System.Text;
using System.Text.Json;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace TESCWatchDog;

public partial class Form1 : Form
{
    private readonly IMqttClient mqttClient;
    private readonly object dailyLogLock = new();
    private readonly string dailyLogDirectory = Path.Combine(AppContext.BaseDirectory, "Log");
    private StreamWriter? dailyLogWriter;
    private DateOnly dailyLogDate;
    private bool dailyLogFailureReported;

    public Form1()
    {
        InitializeComponent();
        InitializeDailyLog();
        InitializeDatabaseSettings();

        mqttClient = new MqttFactory().CreateMqttClient();
        mqttClient.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;
        mqttClient.ConnectedAsync += OnConnectedAsync;
        mqttClient.DisconnectedAsync += OnDisconnectedAsync;

    }

    private async void connectButton_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(portTextBox.Text, out var port) || port is < 1 or > 65535)
        {
            MessageBox.Show("Port 必須是 1 到 65535 的數字。", "設定錯誤",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(hostTextBox.Text))
        {
            MessageBox.Show("Broker 不可空白。", "設定錯誤",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        SetConnectingState();

        try
        {
            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithClientId(string.IsNullOrWhiteSpace(clientIdTextBox.Text)
                    ? $"TESCWatchDog-{Guid.NewGuid():N}"
                    : clientIdTextBox.Text.Trim())
                .WithTcpServer(hostTextBox.Text.Trim(), port)
                .WithCleanSession()
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(30));

            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await mqttClient.ConnectAsync(optionsBuilder.Build(), timeout.Token);
            if (!string.IsNullOrWhiteSpace(topicTextBox.Text))
            {
            var subscription = await mqttClient.SubscribeAsync(
                new MqttTopicFilterBuilder()
                    .WithTopic(topicTextBox.Text.Trim())
                    .WithAtLeastOnceQoS()
                    .Build(),
                timeout.Token);
            if (subscription.Items.Any(item => (int)item.ResultCode >= 128))
                throw new InvalidOperationException("Broker 拒絕訂閱 Topic。");
            }
        }
        catch (Exception ex)
        {
            AppendLog($"連線失敗：{ex.Message}");
            try
            {
                if (mqttClient.IsConnected) await mqttClient.DisconnectAsync();
            }
            catch (Exception disconnectError) { AppendLog($"清理 MQTT 連線失敗：{disconnectError.Message}"); }
            if (IsDisposed || Disposing) return;
            SetDisconnectedState();
        }
    }

    private async void disconnectButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (mqttClient.IsConnected)
            {
                await mqttClient.DisconnectAsync();
            }
        }
        catch (Exception ex)
        {
            AppendLog($"中斷連線時發生錯誤：{ex.Message}");
        }
        finally
        {
            if (!IsDisposed && !Disposing) SetDisconnectedState();
        }
    }

    private async void publishButton_Click(object sender, EventArgs e)
    {
        if (!mqttClient.IsConnected)
        {
            MessageBox.Show("請先連線到 MQTT Broker。", "尚未連線",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var topic = publishTopicTextBox.Text.Trim();
        var payload = publishPayloadTextBox.Text;
        if (string.IsNullOrWhiteSpace(topic) || topic.Contains('+') || topic.Contains('#'))
        {
            MessageBox.Show("發布 Topic 不可空白，也不可包含 + 或 # 萬用字元。", "Topic 錯誤",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (validateJsonCheckBox.Checked)
        {
            try
            {
                using var _ = JsonDocument.Parse(payload);
            }
            catch (JsonException ex)
            {
                MessageBox.Show($"JSON 格式錯誤：{ex.Message}", "訊息格式錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        try
        {
            publishButton.Enabled = false;
            var qos = (MqttQualityOfServiceLevel)qosComboBox.SelectedIndex;
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(qos)
                .WithRetainFlag(retainCheckBox.Checked)
                .Build();

            using var publishTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            var result = await PublishWithReceiptAsync(message, publishTimeout.Token);
            if ((int)result.ReasonCode >= 128) throw new InvalidOperationException($"Broker 拒絕發布：{result.ReasonCode}");
            AppendLog($"已發布 [{topic}] QoS {(int)qos}, {Encoding.UTF8.GetByteCount(payload)} bytes");
        }
        catch (Exception ex)
        {
            AppendLog($"發布失敗：{ex.Message}");
        }
        finally
        {
            if (!IsDisposed && !Disposing) publishButton.Enabled = mqttClient.IsConnected;
        }
    }

    private Task OnConnectedAsync(MqttClientConnectedEventArgs args)
    {
        PostUi(() =>
        {
            statusLabel.Text = "已連線";
            statusLabel.ForeColor = Color.ForestGreen;
            connectButton.Enabled = false;
            disconnectButton.Enabled = true;
            publishButton.Enabled = true;
            pushAlertButton.Enabled = true;
            pushRecoveryButton.Enabled = true;
            AppendLog($"已連線至 {hostTextBox.Text}:{portTextBox.Text}");
        });
        return Task.CompletedTask;
    }

    private Task OnDisconnectedAsync(MqttClientDisconnectedEventArgs args)
    {
        DisconnectReceipts();
        PostUi(() =>
        {
            SetDisconnectedState();
            AppendLog(args.ClientWasConnected
                ? $"連線已中斷：{args.Reason}"
                : "未建立 MQTT 連線。");
        });
        return Task.CompletedTask;
    }

    private Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs args)
    {
        MatchReceipt(args.ApplicationMessage);
        var payload = args.ApplicationMessage.PayloadSegment.Count == 0
            ? string.Empty
            : Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);

        PostUi(() =>
        {
            AppendLog($"MQTT 接收 [{args.ApplicationMessage.Topic}]；內容：{payload}");
        });
        return Task.CompletedTask;
    }

    private void SetConnectingState()
    {
        hostTextBox.Enabled = portTextBox.Enabled = false;
        statusLabel.Text = "連線中...";
        statusLabel.ForeColor = Color.DarkOrange;
        connectButton.Enabled = false;
        disconnectButton.Enabled = false;
        AppendLog($"正在連線至 {hostTextBox.Text}:{portTextBox.Text}...");
    }

    private void SetDisconnectedState()
    {
        hostTextBox.Enabled = portTextBox.Enabled = true;
        statusLabel.Text = "未連線";
        statusLabel.ForeColor = Color.Firebrick;
        connectButton.Enabled = true;
        disconnectButton.Enabled = false;
        publishButton.Enabled = false;
        pushAlertButton.Enabled = false;
        pushRecoveryButton.Enabled = false;
    }

    private void AppendLog(string message)
    {
        if (IsDisposed || Disposing) return;
        if (logTextBox.TextLength > 150000) logTextBox.Clear();
        var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  {message}{Environment.NewLine}";
        logTextBox.AppendText(line);
        WriteDailyLog(line);
    }

    private void InitializeDailyLog()
    {
        try
        {
            Directory.CreateDirectory(dailyLogDirectory);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            dailyLogFailureReported = true;
            logTextBox.AppendText($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  無法建立每日 Log 資料夾：{ex.Message}{Environment.NewLine}");
        }
    }

    private void WriteDailyLog(string line)
    {
        if (dailyLogFailureReported) return;
        try
        {
            lock (dailyLogLock)
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                if (dailyLogWriter == null || dailyLogDate != today)
                {
                    dailyLogWriter?.Dispose();
                    dailyLogDate = today;
                    var path = Path.Combine(dailyLogDirectory, $"{today:yyyy-MM-dd}.txt");
                    var stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read);
                    dailyLogWriter = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };
                }
                dailyLogWriter.Write(line);
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            dailyLogFailureReported = true;
            logTextBox.AppendText($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  每日 Log 寫入失敗：{ex.Message}{Environment.NewLine}");
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        // 先取消非同步工作，再釋放 MQTT；晚到的 UI 回呼由 PostUi 忽略。
        monitorCancellation?.Cancel();
        receiptLifetime.Cancel();
        DisconnectReceipts();
        lock (dailyLogLock)
        {
            dailyLogWriter?.Dispose();
            dailyLogWriter = null;
        }
        mqttClient.Dispose();
        base.OnFormClosed(e);
    }

    private void PostUi(Action action)
    {
        // MQTTnet 回呼可能在背景執行緒，所有控制項操作都切回 UI 執行緒。
        if (!IsHandleCreated || IsDisposed || Disposing) return;
        try { BeginInvoke(() => { if (!IsDisposed && !Disposing) action(); }); }
        catch (InvalidOperationException) { }
    }
}
