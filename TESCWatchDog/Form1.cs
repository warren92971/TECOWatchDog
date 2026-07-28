using System.Text;
using System.Text.Json;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace TESCWatchDog;

public partial class Form1 : Form
{
    private readonly IMqttClient mqttClient;
    private readonly System.Windows.Forms.Timer watchdogTimer;
    private DateTimeOffset? lastMessageReceivedAt;
    private bool timeoutAlertSent;
    private bool timeoutAlertPublishing;

    public Form1()
    {
        InitializeComponent();

        mqttClient = new MqttFactory().CreateMqttClient();
        mqttClient.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;
        mqttClient.ConnectedAsync += OnConnectedAsync;
        mqttClient.DisconnectedAsync += OnDisconnectedAsync;

        watchdogTimer = new System.Windows.Forms.Timer { Interval = 1000 };
        watchdogTimer.Tick += watchdogTimer_Tick;
    }

    private async void connectButton_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(portTextBox.Text, out var port) || port is < 1 or > 65535)
        {
            MessageBox.Show("Port 必須是 1 到 65535 的數字。", "設定錯誤",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(hostTextBox.Text) ||
            string.IsNullOrWhiteSpace(topicTextBox.Text))
        {
            MessageBox.Show("Broker 與 Topic 不可空白。", "設定錯誤",
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

            if (!string.IsNullOrWhiteSpace(usernameTextBox.Text))
            {
                optionsBuilder.WithCredentials(usernameTextBox.Text, passwordTextBox.Text);
            }

            if (tlsCheckBox.Checked)
            {
                optionsBuilder.WithTlsOptions(options => options.UseTls());
            }

            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await mqttClient.ConnectAsync(optionsBuilder.Build(), timeout.Token);
            await mqttClient.SubscribeAsync(
                new MqttTopicFilterBuilder()
                    .WithTopic(topicTextBox.Text.Trim())
                    .WithAtLeastOnceQoS()
                    .Build(),
                timeout.Token);

            lastMessageReceivedAt = DateTimeOffset.Now;
            timeoutAlertSent = false;
            watchdogTimer.Start();
        }
        catch (Exception ex)
        {
            AppendLog($"連線失敗：{ex.Message}");
            if (mqttClient.IsConnected)
            {
                await mqttClient.DisconnectAsync();
            }
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
            SetDisconnectedState();
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

            await mqttClient.PublishAsync(message);
            AppendLog($"已發布 [{topic}] QoS {(int)qos}, {Encoding.UTF8.GetByteCount(payload)} bytes");
        }
        catch (Exception ex)
        {
            AppendLog($"發布失敗：{ex.Message}");
        }
        finally
        {
            publishButton.Enabled = mqttClient.IsConnected;
        }
    }

    private async void watchdogTimer_Tick(object? sender, EventArgs e)
    {
        if (!autoAlertCheckBox.Checked || !mqttClient.IsConnected ||
            lastMessageReceivedAt is null || timeoutAlertSent || timeoutAlertPublishing)
        {
            return;
        }

        var elapsed = DateTimeOffset.Now - lastMessageReceivedAt.Value;
        if (elapsed.TotalSeconds < (double)timeoutSecondsNumeric.Value)
        {
            return;
        }

        timeoutAlertPublishing = true;
        try
        {
            var topic = alertTopicTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(topic) || topic.Contains('+') || topic.Contains('#'))
            {
                AppendLog("逾時警報未發布：警報 Topic 不可空白或包含萬用字元。");
                timeoutAlertSent = true;
                return;
            }

            var payload = alertPayloadTextBox.Text
                .Replace("{timestamp}", DateTimeOffset.Now.ToString("O"), StringComparison.Ordinal)
                .Replace("{seconds}", ((int)elapsed.TotalSeconds).ToString(), StringComparison.Ordinal)
                .Replace("{lastReceived}", lastMessageReceivedAt.Value.ToString("O"), StringComparison.Ordinal);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await mqttClient.PublishAsync(message);
            timeoutAlertSent = true;
            AppendLog($"已自動發布無資料警報 [{topic}]，已 {elapsed.TotalSeconds:F0} 秒未收到資料。");
        }
        catch (Exception ex)
        {
            AppendLog($"自動發布警報失敗：{ex.Message}");
        }
        finally
        {
            timeoutAlertPublishing = false;
        }
    }

    private Task OnConnectedAsync(MqttClientConnectedEventArgs args)
    {
        BeginInvoke(() =>
        {
            statusLabel.Text = "已連線";
            statusLabel.ForeColor = Color.ForestGreen;
            connectButton.Enabled = false;
            disconnectButton.Enabled = true;
            publishButton.Enabled = true;
            AppendLog($"已連線至 {hostTextBox.Text}:{portTextBox.Text}");
        });
        return Task.CompletedTask;
    }

    private Task OnDisconnectedAsync(MqttClientDisconnectedEventArgs args)
    {
        BeginInvoke(() =>
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
        var payload = args.ApplicationMessage.PayloadSegment.Count == 0
            ? string.Empty
            : Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);

        BeginInvoke(() =>
        {
            lastMessageReceivedAt = DateTimeOffset.Now;
            timeoutAlertSent = false;
            AppendLog($"[{args.ApplicationMessage.Topic}] {payload}");
        });
        return Task.CompletedTask;
    }

    private void SetConnectingState()
    {
        statusLabel.Text = "連線中...";
        statusLabel.ForeColor = Color.DarkOrange;
        connectButton.Enabled = false;
        disconnectButton.Enabled = false;
        AppendLog($"正在連線至 {hostTextBox.Text}:{portTextBox.Text}...");
    }

    private void SetDisconnectedState()
    {
        watchdogTimer.Stop();
        lastMessageReceivedAt = null;
        timeoutAlertSent = false;
        statusLabel.Text = "未連線";
        statusLabel.ForeColor = Color.Firebrick;
        connectButton.Enabled = true;
        disconnectButton.Enabled = false;
        publishButton.Enabled = false;
    }

    private void AppendLog(string message)
    {
        logTextBox.AppendText($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  {message}{Environment.NewLine}");
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        watchdogTimer.Dispose();
        mqttClient.Dispose();
        base.OnFormClosed(e);
    }
}
