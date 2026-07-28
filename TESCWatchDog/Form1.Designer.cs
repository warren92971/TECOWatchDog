namespace TESCWatchDog
{
    partial class Form1
    {
        private Label hostLabel;
        private TextBox hostTextBox;
        private Label portLabel;
        private TextBox portTextBox;
        private Label clientIdLabel;
        private TextBox clientIdTextBox;
        private Label topicLabel;
        private TextBox topicTextBox;
        private Label usernameLabel;
        private TextBox usernameTextBox;
        private Label passwordLabel;
        private TextBox passwordTextBox;
        private CheckBox tlsCheckBox;
        private Button connectButton;
        private Button disconnectButton;
        private Label statusLabel;
        private GroupBox publishGroupBox;
        private Label publishTopicLabel;
        private TextBox publishTopicTextBox;
        private Label qosLabel;
        private ComboBox qosComboBox;
        private CheckBox retainCheckBox;
        private CheckBox validateJsonCheckBox;
        private TextBox publishPayloadTextBox;
        private Button publishButton;
        private GroupBox watchdogGroupBox;
        private CheckBox autoAlertCheckBox;
        private Label timeoutSecondsLabel;
        private NumericUpDown timeoutSecondsNumeric;
        private Label alertTopicLabel;
        private TextBox alertTopicTextBox;
        private Label alertPayloadLabel;
        private TextBox alertPayloadTextBox;
        private TextBox logTextBox;
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            hostLabel = new Label();
            hostTextBox = new TextBox();
            portLabel = new Label();
            portTextBox = new TextBox();
            clientIdLabel = new Label();
            clientIdTextBox = new TextBox();
            topicLabel = new Label();
            topicTextBox = new TextBox();
            usernameLabel = new Label();
            usernameTextBox = new TextBox();
            passwordLabel = new Label();
            passwordTextBox = new TextBox();
            tlsCheckBox = new CheckBox();
            connectButton = new Button();
            disconnectButton = new Button();
            statusLabel = new Label();
            publishGroupBox = new GroupBox();
            publishTopicLabel = new Label();
            publishTopicTextBox = new TextBox();
            qosLabel = new Label();
            qosComboBox = new ComboBox();
            retainCheckBox = new CheckBox();
            validateJsonCheckBox = new CheckBox();
            publishPayloadTextBox = new TextBox();
            publishButton = new Button();
            watchdogGroupBox = new GroupBox();
            autoAlertCheckBox = new CheckBox();
            timeoutSecondsLabel = new Label();
            timeoutSecondsNumeric = new NumericUpDown();
            alertTopicLabel = new Label();
            alertTopicTextBox = new TextBox();
            alertPayloadLabel = new Label();
            alertPayloadTextBox = new TextBox();
            logTextBox = new TextBox();
            publishGroupBox.SuspendLayout();
            watchdogGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)timeoutSecondsNumeric).BeginInit();
            SuspendLayout();
            // 
            // hostLabel
            // 
            hostLabel.AutoSize = true;
            hostLabel.Location = new Point(24, 25);
            hostLabel.Name = "hostLabel";
            hostLabel.Size = new Size(80, 19);
            hostLabel.Text = "MQTT Broker";
            // 
            // hostTextBox
            // 
            hostTextBox.Location = new Point(128, 21);
            hostTextBox.Name = "hostTextBox";
            hostTextBox.Size = new Size(220, 27);
            hostTextBox.Text = "109.123.238.225";
            // 
            // portLabel
            // 
            portLabel.AutoSize = true;
            portLabel.Location = new Point(370, 25);
            portLabel.Name = "portLabel";
            portLabel.Text = "Port";
            // 
            // portTextBox
            // 
            portTextBox.Location = new Point(418, 21);
            portTextBox.Name = "portTextBox";
            portTextBox.Size = new Size(75, 27);
            portTextBox.Text = "1883";
            // 
            // tlsCheckBox
            // 
            tlsCheckBox.AutoSize = true;
            tlsCheckBox.Location = new Point(516, 23);
            tlsCheckBox.Name = "tlsCheckBox";
            tlsCheckBox.Text = "TLS";
            // 
            // clientIdLabel
            // 
            clientIdLabel.AutoSize = true;
            clientIdLabel.Location = new Point(24, 67);
            clientIdLabel.Name = "clientIdLabel";
            clientIdLabel.Text = "Client ID";
            // 
            // clientIdTextBox
            // 
            clientIdTextBox.Location = new Point(128, 63);
            clientIdTextBox.Name = "clientIdTextBox";
            clientIdTextBox.Size = new Size(465, 27);
            clientIdTextBox.Text = "TESCWatchDog";
            // 
            // topicLabel
            // 
            topicLabel.AutoSize = true;
            topicLabel.Location = new Point(24, 109);
            topicLabel.Name = "topicLabel";
            topicLabel.Text = "訂閱 Topic";
            // 
            // topicTextBox
            // 
            topicTextBox.Location = new Point(128, 105);
            topicTextBox.Name = "topicTextBox";
            topicTextBox.Size = new Size(465, 27);
            topicTextBox.Text = "zhongli/+/compressor/+/telemetry";
            // 
            // usernameLabel
            // 
            usernameLabel.AutoSize = true;
            usernameLabel.Location = new Point(24, 151);
            usernameLabel.Name = "usernameLabel";
            usernameLabel.Text = "帳號";
            // 
            // usernameTextBox
            // 
            usernameTextBox.Location = new Point(128, 147);
            usernameTextBox.Name = "usernameTextBox";
            usernameTextBox.Size = new Size(180, 27);
            // 
            // passwordLabel
            // 
            passwordLabel.AutoSize = true;
            passwordLabel.Location = new Point(329, 151);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Text = "密碼";
            // 
            // passwordTextBox
            // 
            passwordTextBox.Location = new Point(378, 147);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.PasswordChar = '●';
            passwordTextBox.Size = new Size(215, 27);
            // 
            // connectButton
            // 
            connectButton.Location = new Point(620, 21);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(120, 40);
            connectButton.Text = "連線並訂閱";
            connectButton.UseVisualStyleBackColor = true;
            connectButton.Click += connectButton_Click;
            // 
            // disconnectButton
            // 
            disconnectButton.Enabled = false;
            disconnectButton.Location = new Point(620, 72);
            disconnectButton.Name = "disconnectButton";
            disconnectButton.Size = new Size(120, 40);
            disconnectButton.Text = "中斷連線";
            disconnectButton.UseVisualStyleBackColor = true;
            disconnectButton.Click += disconnectButton_Click;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.ForeColor = Color.Firebrick;
            statusLabel.Location = new Point(620, 151);
            statusLabel.Name = "statusLabel";
            statusLabel.Text = "未連線";
            // 
            // publishGroupBox
            // 
            publishGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            publishGroupBox.Controls.Add(publishButton);
            publishGroupBox.Controls.Add(publishPayloadTextBox);
            publishGroupBox.Controls.Add(validateJsonCheckBox);
            publishGroupBox.Controls.Add(retainCheckBox);
            publishGroupBox.Controls.Add(qosComboBox);
            publishGroupBox.Controls.Add(qosLabel);
            publishGroupBox.Controls.Add(publishTopicTextBox);
            publishGroupBox.Controls.Add(publishTopicLabel);
            publishGroupBox.Location = new Point(24, 190);
            publishGroupBox.Name = "publishGroupBox";
            publishGroupBox.Size = new Size(716, 260);
            publishGroupBox.Text = "發布訊息";
            // 
            // publishTopicLabel
            // 
            publishTopicLabel.AutoSize = true;
            publishTopicLabel.Location = new Point(14, 34);
            publishTopicLabel.Name = "publishTopicLabel";
            publishTopicLabel.Text = "發布 Topic";
            // 
            // publishTopicTextBox
            // 
            publishTopicTextBox.Location = new Point(104, 30);
            publishTopicTextBox.Name = "publishTopicTextBox";
            publishTopicTextBox.Size = new Size(360, 27);
            publishTopicTextBox.Text = "zhongli/zone1/compressor/c1/telemetry";
            // 
            // qosLabel
            // 
            qosLabel.AutoSize = true;
            qosLabel.Location = new Point(478, 34);
            qosLabel.Name = "qosLabel";
            qosLabel.Text = "QoS";
            // 
            // qosComboBox
            // 
            qosComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            qosComboBox.FormattingEnabled = true;
            qosComboBox.Items.AddRange(new object[] { "0", "1", "2" });
            qosComboBox.Location = new Point(520, 30);
            qosComboBox.Name = "qosComboBox";
            qosComboBox.Size = new Size(52, 27);
            qosComboBox.SelectedIndex = 1;
            // 
            // retainCheckBox
            // 
            retainCheckBox.AutoSize = true;
            retainCheckBox.Location = new Point(586, 32);
            retainCheckBox.Name = "retainCheckBox";
            retainCheckBox.Text = "Retain";
            // 
            // validateJsonCheckBox
            // 
            validateJsonCheckBox.AutoSize = true;
            validateJsonCheckBox.Checked = true;
            validateJsonCheckBox.CheckState = CheckState.Checked;
            validateJsonCheckBox.Location = new Point(14, 69);
            validateJsonCheckBox.Name = "validateJsonCheckBox";
            validateJsonCheckBox.Text = "發布前驗證 JSON";
            // 
            // publishPayloadTextBox
            // 
            publishPayloadTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            publishPayloadTextBox.Font = new Font("Consolas", 9F);
            publishPayloadTextBox.Location = new Point(14, 98);
            publishPayloadTextBox.Multiline = true;
            publishPayloadTextBox.Name = "publishPayloadTextBox";
            publishPayloadTextBox.ScrollBars = ScrollBars.Both;
            publishPayloadTextBox.Size = new Size(570, 146);
            publishPayloadTextBox.Text = "{\r\n  \"compressor_drive_type\": \"VSD\",\r\n  \"timestamp\": \"2026-06-01T15:46:00+08:00\"\r\n}";
            publishPayloadTextBox.WordWrap = false;
            // 
            // publishButton
            // 
            publishButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            publishButton.Enabled = false;
            publishButton.Location = new Point(598, 196);
            publishButton.Name = "publishButton";
            publishButton.Size = new Size(104, 48);
            publishButton.Text = "發布";
            publishButton.UseVisualStyleBackColor = true;
            publishButton.Click += publishButton_Click;
            // 
            // watchdogGroupBox
            // 
            watchdogGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            watchdogGroupBox.Controls.Add(alertPayloadTextBox);
            watchdogGroupBox.Controls.Add(alertPayloadLabel);
            watchdogGroupBox.Controls.Add(alertTopicTextBox);
            watchdogGroupBox.Controls.Add(alertTopicLabel);
            watchdogGroupBox.Controls.Add(timeoutSecondsNumeric);
            watchdogGroupBox.Controls.Add(timeoutSecondsLabel);
            watchdogGroupBox.Controls.Add(autoAlertCheckBox);
            watchdogGroupBox.Location = new Point(24, 466);
            watchdogGroupBox.Name = "watchdogGroupBox";
            watchdogGroupBox.Size = new Size(716, 172);
            watchdogGroupBox.Text = "無資料逾時警報";
            // 
            // autoAlertCheckBox
            // 
            autoAlertCheckBox.AutoSize = true;
            autoAlertCheckBox.Checked = true;
            autoAlertCheckBox.CheckState = CheckState.Checked;
            autoAlertCheckBox.Location = new Point(14, 30);
            autoAlertCheckBox.Name = "autoAlertCheckBox";
            autoAlertCheckBox.Text = "啟用";
            // 
            // timeoutSecondsLabel
            // 
            timeoutSecondsLabel.AutoSize = true;
            timeoutSecondsLabel.Location = new Point(91, 31);
            timeoutSecondsLabel.Name = "timeoutSecondsLabel";
            timeoutSecondsLabel.Text = "未收到資料（秒）";
            // 
            // timeoutSecondsNumeric
            // 
            timeoutSecondsNumeric.Location = new Point(181, 27);
            timeoutSecondsNumeric.Maximum = new decimal(new int[] { 86400, 0, 0, 0 });
            timeoutSecondsNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            timeoutSecondsNumeric.Name = "timeoutSecondsNumeric";
            timeoutSecondsNumeric.Size = new Size(90, 27);
            timeoutSecondsNumeric.Value = new decimal(new int[] { 30, 0, 0, 0 });
            // 
            // alertTopicLabel
            // 
            alertTopicLabel.AutoSize = true;
            alertTopicLabel.Location = new Point(14, 71);
            alertTopicLabel.Name = "alertTopicLabel";
            alertTopicLabel.Text = "警報 Topic";
            // 
            // alertTopicTextBox
            // 
            alertTopicTextBox.Location = new Point(104, 67);
            alertTopicTextBox.Name = "alertTopicTextBox";
            alertTopicTextBox.Size = new Size(598, 27);
            alertTopicTextBox.Text = "tesc/watchdog/alert";
            // 
            // alertPayloadLabel
            // 
            alertPayloadLabel.AutoSize = true;
            alertPayloadLabel.Location = new Point(14, 112);
            alertPayloadLabel.Name = "alertPayloadLabel";
            alertPayloadLabel.Text = "警報訊息";
            // 
            // alertPayloadTextBox
            // 
            alertPayloadTextBox.Location = new Point(104, 108);
            alertPayloadTextBox.Multiline = true;
            alertPayloadTextBox.Name = "alertPayloadTextBox";
            alertPayloadTextBox.ScrollBars = ScrollBars.Vertical;
            alertPayloadTextBox.Size = new Size(598, 50);
            alertPayloadTextBox.Text = "{\"status\":\"no_data\",\"timestamp\":\"{timestamp}\",\"seconds\":{seconds},\"last_received\":\"{lastReceived}\"}";
            // 
            // logTextBox
            // 
            logTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            logTextBox.BackColor = Color.FromArgb(25, 25, 25);
            logTextBox.Font = new Font("Consolas", 10F);
            logTextBox.ForeColor = Color.Gainsboro;
            logTextBox.Location = new Point(24, 654);
            logTextBox.Multiline = true;
            logTextBox.Name = "logTextBox";
            logTextBox.ReadOnly = true;
            logTextBox.ScrollBars = ScrollBars.Both;
            logTextBox.Size = new Size(716, 236);
            logTextBox.WordWrap = false;
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(766, 914);
            Controls.Add(logTextBox);
            Controls.Add(watchdogGroupBox);
            Controls.Add(publishGroupBox);
            Controls.Add(statusLabel);
            Controls.Add(disconnectButton);
            Controls.Add(connectButton);
            Controls.Add(tlsCheckBox);
            Controls.Add(passwordTextBox);
            Controls.Add(passwordLabel);
            Controls.Add(usernameTextBox);
            Controls.Add(usernameLabel);
            Controls.Add(topicTextBox);
            Controls.Add(topicLabel);
            Controls.Add(clientIdTextBox);
            Controls.Add(clientIdLabel);
            Controls.Add(portTextBox);
            Controls.Add(portLabel);
            Controls.Add(hostTextBox);
            Controls.Add(hostLabel);
            MinimumSize = new Size(782, 800);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "TESC WatchDog - MQTT 監控";
            publishGroupBox.ResumeLayout(false);
            publishGroupBox.PerformLayout();
            watchdogGroupBox.ResumeLayout(false);
            watchdogGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)timeoutSecondsNumeric).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
