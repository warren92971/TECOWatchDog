namespace TESCWatchDog;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel shell;
    private TabControl tabs;
    private TabPage dbPage;
    private Panel toolbar;
    private Button startMonitor;
    private Button stopMonitor;
    private Label dbStatus;
    private Label hostLabel;
    private TextBox hostTextBox;
    private Label portLabel;
    private TextBox portTextBox;
    private Button connectButton;
    private Button disconnectButton;
    private Label statusLabel;
    private Panel monitorSettings;
    private GroupBox cdbGroup;
    private GroupBox notificationGroup;
    private Label dbServerLabel;
    private TextBox dbServer;
    private Label dbNameLabel;
    private TextBox dbName;
    private Label dbSchemaLabel;
    private TextBox dbSchema;
    private Label dbUserLabel;
    private TextBox dbUser;
    private Label dbPasswordLabel;
    private TextBox dbPassword;
    private Label authLabel;
    private CheckBox integratedAuth;
    private CheckBox trustDbCertificate;
    private Label pollSecondsLabel;
    private NumericUpDown pollSeconds;
    private Label timeoutSecondsNumericLabel;
    private NumericUpDown timeoutSecondsNumeric;
    private Label targetLabel;
    private CheckBox autoAlertCheckBox;
    private Label alertTopicTextBoxLabel;
    private TextBox alertTopicTextBox;
    private Label alertPayloadTextBoxLabel;
    private TextBox alertPayloadTextBox;
    private Button pushAlertButton;
    private CheckBox recoveryEnabled;
    private Label recoveryTopicLabel;
    private TextBox recoveryTopic;
    private Label recoveryPayloadLabel;
    private TextBox recoveryPayload;
    private Button pushRecoveryButton;
    private Label variablesLabel;
    private TextBox logTextBox;
    private Panel manualPanel;
    private TextBox clientIdTextBox;
    private TextBox topicTextBox;
    private TextBox publishTopicTextBox;
    private TextBox publishPayloadTextBox;
    private ComboBox qosComboBox;
    private CheckBox retainCheckBox;
    private CheckBox validateJsonCheckBox;
    private Button publishButton;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }
    #region Windows Form Designer generated code
    private void InitializeComponent()
    {
        shell = new TableLayoutPanel();
        tabs = new TabControl();
        dbPage = new TabPage();
        toolbar = new Panel();
        startMonitor = new Button();
        stopMonitor = new Button();
        dbStatus = new Label();
        hostLabel = new Label();
        hostTextBox = new TextBox();
        portLabel = new Label();
        portTextBox = new TextBox();
        connectButton = new Button();
        disconnectButton = new Button();
        statusLabel = new Label();
        monitorSettings = new Panel();
        cdbGroup = new GroupBox();
        dbServerLabel = new Label();
        dbServer = new TextBox();
        dbNameLabel = new Label();
        dbName = new TextBox();
        dbSchemaLabel = new Label();
        dbSchema = new TextBox();
        dbUserLabel = new Label();
        dbUser = new TextBox();
        dbPasswordLabel = new Label();
        dbPassword = new TextBox();
        authLabel = new Label();
        integratedAuth = new CheckBox();
        trustDbCertificate = new CheckBox();
        pollSecondsLabel = new Label();
        pollSeconds = new NumericUpDown();
        timeoutSecondsNumericLabel = new Label();
        timeoutSecondsNumeric = new NumericUpDown();
        targetLabel = new Label();
        notificationGroup = new GroupBox();
        autoAlertCheckBox = new CheckBox();
        alertTopicTextBoxLabel = new Label();
        alertTopicTextBox = new TextBox();
        alertPayloadTextBoxLabel = new Label();
        alertPayloadTextBox = new TextBox();
        pushAlertButton = new Button();
        recoveryEnabled = new CheckBox();
        recoveryTopicLabel = new Label();
        recoveryTopic = new TextBox();
        recoveryPayloadLabel = new Label();
        recoveryPayload = new TextBox();
        pushRecoveryButton = new Button();
        variablesLabel = new Label();
        logTextBox = new TextBox();
        manualPanel = new Panel();
        clientIdTextBox = new TextBox();
        topicTextBox = new TextBox();
        publishTopicTextBox = new TextBox();
        publishPayloadTextBox = new TextBox();
        qosComboBox = new ComboBox();
        retainCheckBox = new CheckBox();
        validateJsonCheckBox = new CheckBox();
        publishButton = new Button();
        shell.SuspendLayout();
        tabs.SuspendLayout();
        dbPage.SuspendLayout();
        toolbar.SuspendLayout();
        monitorSettings.SuspendLayout();
        cdbGroup.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pollSeconds).BeginInit();
        ((System.ComponentModel.ISupportInitialize)timeoutSecondsNumeric).BeginInit();
        notificationGroup.SuspendLayout();
        manualPanel.SuspendLayout();
        SuspendLayout();
        // 
        // shell
        // 
        shell.ColumnCount = 1;
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        shell.Controls.Add(tabs, 0, 0);
        shell.Controls.Add(logTextBox, 0, 1);
        shell.Dock = DockStyle.Fill;
        shell.Location = new Point(0, 0);
        shell.Name = "shell";
        shell.RowCount = 2;
        shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        shell.RowStyles.Add(new RowStyle(SizeType.Absolute, 240F));
        shell.Size = new Size(1100, 779);
        shell.TabIndex = 0;
        // 
        // tabs
        // 
        tabs.Controls.Add(dbPage);
        tabs.Dock = DockStyle.Fill;
        tabs.Location = new Point(3, 3);
        tabs.Name = "tabs";
        tabs.SelectedIndex = 0;
        tabs.Size = new Size(1094, 553);
        tabs.TabIndex = 0;
        // 
        // dbPage
        // 
        dbPage.AutoScroll = true;
        dbPage.Controls.Add(toolbar);
        dbPage.Controls.Add(monitorSettings);
        dbPage.Location = new Point(4, 24);
        dbPage.Name = "dbPage";
        dbPage.Size = new Size(1086, 525);
        dbPage.TabIndex = 0;
        dbPage.Text = "CDB 監控／通知";
        // 
        // toolbar
        // 
        toolbar.Controls.Add(startMonitor);
        toolbar.Controls.Add(stopMonitor);
        toolbar.Controls.Add(dbStatus);
        toolbar.Controls.Add(hostLabel);
        toolbar.Controls.Add(hostTextBox);
        toolbar.Controls.Add(portLabel);
        toolbar.Controls.Add(portTextBox);
        toolbar.Controls.Add(connectButton);
        toolbar.Controls.Add(disconnectButton);
        toolbar.Controls.Add(statusLabel);
        toolbar.Location = new Point(6, 6);
        toolbar.Name = "toolbar";
        toolbar.Size = new Size(1040, 82);
        toolbar.TabIndex = 0;
        // 
        // startMonitor
        // 
        startMonitor.Location = new Point(4, 2);
        startMonitor.Name = "startMonitor";
        startMonitor.Size = new Size(86, 30);
        startMonitor.TabIndex = 0;
        startMonitor.Text = "開始監控";
        startMonitor.Click += StartMonitor_Click;
        // 
        // stopMonitor
        // 
        stopMonitor.Enabled = false;
        stopMonitor.Location = new Point(96, 2);
        stopMonitor.Name = "stopMonitor";
        stopMonitor.Size = new Size(86, 30);
        stopMonitor.TabIndex = 1;
        stopMonitor.Text = "停止監控";
        stopMonitor.Click += StopMonitor_Click;
        // 
        // dbStatus
        // 
        dbStatus.Location = new Point(192, 2);
        dbStatus.Name = "dbStatus";
        dbStatus.Size = new Size(650, 30);
        dbStatus.TabIndex = 2;
        dbStatus.Text = "尚未開始監控";
        dbStatus.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // hostLabel
        // 
        hostLabel.Location = new Point(4, 42);
        hostLabel.Name = "hostLabel";
        hostLabel.Size = new Size(88, 30);
        hostLabel.TabIndex = 3;
        hostLabel.Text = "MQTT Broker";
        hostLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // hostTextBox
        // 
        hostTextBox.Location = new Point(98, 46);
        hostTextBox.Name = "hostTextBox";
        hostTextBox.Size = new Size(150, 23);
        hostTextBox.TabIndex = 4;
        hostTextBox.Text = "109.123.238.225";
        // 
        // portLabel
        // 
        portLabel.Location = new Point(254, 42);
        portLabel.Name = "portLabel";
        portLabel.Size = new Size(42, 30);
        portLabel.TabIndex = 5;
        portLabel.Text = "Port";
        portLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // portTextBox
        // 
        portTextBox.Location = new Point(302, 46);
        portTextBox.Name = "portTextBox";
        portTextBox.Size = new Size(60, 23);
        portTextBox.TabIndex = 6;
        portTextBox.Text = "1883";
        // 
        // connectButton
        // 
        connectButton.Location = new Point(374, 42);
        connectButton.Name = "connectButton";
        connectButton.Size = new Size(100, 30);
        connectButton.TabIndex = 7;
        connectButton.Text = "連線 MQTT";
        connectButton.Click += connectButton_Click;
        // 
        // disconnectButton
        // 
        disconnectButton.Enabled = false;
        disconnectButton.Location = new Point(482, 42);
        disconnectButton.Name = "disconnectButton";
        disconnectButton.Size = new Size(100, 30);
        disconnectButton.TabIndex = 8;
        disconnectButton.Text = "中斷連線";
        disconnectButton.Click += disconnectButton_Click;
        // 
        // statusLabel
        // 
        statusLabel.ForeColor = Color.Firebrick;
        statusLabel.Location = new Point(590, 42);
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(100, 30);
        statusLabel.TabIndex = 9;
        statusLabel.Text = "未連線";
        statusLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // monitorSettings
        // 
        monitorSettings.Controls.Add(cdbGroup);
        monitorSettings.Controls.Add(notificationGroup);
        monitorSettings.Location = new Point(6, 94);
        monitorSettings.Name = "monitorSettings";
        monitorSettings.Size = new Size(1056, 407);
        monitorSettings.TabIndex = 1;
        // 
        // cdbGroup
        // 
        cdbGroup.Controls.Add(dbServerLabel);
        cdbGroup.Controls.Add(dbServer);
        cdbGroup.Controls.Add(dbNameLabel);
        cdbGroup.Controls.Add(dbName);
        cdbGroup.Controls.Add(dbSchemaLabel);
        cdbGroup.Controls.Add(dbSchema);
        cdbGroup.Controls.Add(dbUserLabel);
        cdbGroup.Controls.Add(dbUser);
        cdbGroup.Controls.Add(dbPasswordLabel);
        cdbGroup.Controls.Add(dbPassword);
        cdbGroup.Controls.Add(authLabel);
        cdbGroup.Controls.Add(integratedAuth);
        cdbGroup.Controls.Add(trustDbCertificate);
        cdbGroup.Controls.Add(pollSecondsLabel);
        cdbGroup.Controls.Add(pollSeconds);
        cdbGroup.Controls.Add(timeoutSecondsNumericLabel);
        cdbGroup.Controls.Add(timeoutSecondsNumeric);
        cdbGroup.Controls.Add(targetLabel);
        cdbGroup.Location = new Point(0, 0);
        cdbGroup.Name = "cdbGroup";
        cdbGroup.Size = new Size(492, 400);
        cdbGroup.TabIndex = 0;
        cdbGroup.TabStop = false;
        cdbGroup.Text = "CDB 連線與查詢";
        // 
        // dbServerLabel
        // 
        dbServerLabel.Location = new Point(10, 26);
        dbServerLabel.Name = "dbServerLabel";
        dbServerLabel.Size = new Size(140, 27);
        dbServerLabel.TabIndex = 0;
        dbServerLabel.Text = "SQL Server／執行個體";
        dbServerLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // dbServer
        // 
        dbServer.Location = new Point(156, 28);
        dbServer.Name = "dbServer";
        dbServer.Size = new Size(260, 23);
        dbServer.TabIndex = 1;
        dbServer.Text = "140.116.234.18";
        // 
        // dbNameLabel
        // 
        dbNameLabel.Location = new Point(10, 62);
        dbNameLabel.Name = "dbNameLabel";
        dbNameLabel.Size = new Size(140, 27);
        dbNameLabel.TabIndex = 2;
        dbNameLabel.Text = "資料庫";
        dbNameLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // dbName
        // 
        dbName.Location = new Point(156, 64);
        dbName.Name = "dbName";
        dbName.Size = new Size(260, 23);
        dbName.TabIndex = 3;
        dbName.Text = "TECO_AVM3_CDB_Warren_v2";
        // 
        // dbSchemaLabel
        // 
        dbSchemaLabel.Location = new Point(10, 98);
        dbSchemaLabel.Name = "dbSchemaLabel";
        dbSchemaLabel.Size = new Size(140, 27);
        dbSchemaLabel.TabIndex = 4;
        dbSchemaLabel.Text = "Schema";
        dbSchemaLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // dbSchema
        // 
        dbSchema.Location = new Point(156, 100);
        dbSchema.Name = "dbSchema";
        dbSchema.Size = new Size(260, 23);
        dbSchema.TabIndex = 5;
        dbSchema.Text = "dbo";
        // 
        // dbUserLabel
        // 
        dbUserLabel.Location = new Point(10, 170);
        dbUserLabel.Name = "dbUserLabel";
        dbUserLabel.Size = new Size(140, 27);
        dbUserLabel.TabIndex = 6;
        dbUserLabel.Text = "SQL 帳號";
        dbUserLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // dbUser
        // 
        dbUser.Location = new Point(156, 172);
        dbUser.Name = "dbUser";
        dbUser.Size = new Size(260, 23);
        dbUser.TabIndex = 7;
        // 
        // dbPasswordLabel
        // 
        dbPasswordLabel.Location = new Point(10, 206);
        dbPasswordLabel.Name = "dbPasswordLabel";
        dbPasswordLabel.Size = new Size(140, 27);
        dbPasswordLabel.TabIndex = 8;
        dbPasswordLabel.Text = "SQL 密碼";
        dbPasswordLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // dbPassword
        // 
        dbPassword.Location = new Point(156, 208);
        dbPassword.Name = "dbPassword";
        dbPassword.Size = new Size(260, 23);
        dbPassword.TabIndex = 9;
        dbPassword.UseSystemPasswordChar = true;
        // 
        // authLabel
        // 
        authLabel.Location = new Point(10, 134);
        authLabel.Name = "authLabel";
        authLabel.Size = new Size(140, 27);
        authLabel.TabIndex = 10;
        authLabel.Text = "驗證方式";
        authLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // integratedAuth
        // 
        integratedAuth.Location = new Point(156, 134);
        integratedAuth.Name = "integratedAuth";
        integratedAuth.Size = new Size(260, 27);
        integratedAuth.TabIndex = 11;
        integratedAuth.Text = "Windows 驗證";
        integratedAuth.CheckedChanged += IntegratedAuth_CheckedChanged;
        // 
        // trustDbCertificate
        // 
        trustDbCertificate.Location = new Point(18, 244);
        trustDbCertificate.Name = "trustDbCertificate";
        trustDbCertificate.Size = new Size(460, 28);
        trustDbCertificate.TabIndex = 12;
        trustDbCertificate.Text = "信任 SQL Server 憑證（僅可信任內網／自簽憑證）";
        // 
        // pollSecondsLabel
        // 
        pollSecondsLabel.Location = new Point(10, 280);
        pollSecondsLabel.Name = "pollSecondsLabel";
        pollSecondsLabel.Size = new Size(140, 27);
        pollSecondsLabel.TabIndex = 13;
        pollSecondsLabel.Text = "查詢間隔（秒）";
        pollSecondsLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // pollSeconds
        // 
        pollSeconds.Location = new Point(156, 282);
        pollSeconds.Maximum = new decimal(new int[] { 86400, 0, 0, 0 });
        pollSeconds.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        pollSeconds.Name = "pollSeconds";
        pollSeconds.Size = new Size(90, 23);
        pollSeconds.TabIndex = 14;
        pollSeconds.Value = new decimal(new int[] { 10, 0, 0, 0 });
        // 
        // timeoutSecondsNumericLabel
        // 
        timeoutSecondsNumericLabel.Location = new Point(10, 314);
        timeoutSecondsNumericLabel.Name = "timeoutSecondsNumericLabel";
        timeoutSecondsNumericLabel.Size = new Size(140, 27);
        timeoutSecondsNumericLabel.TabIndex = 15;
        timeoutSecondsNumericLabel.Text = "無新資料門檻（秒）";
        timeoutSecondsNumericLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // timeoutSecondsNumeric
        // 
        timeoutSecondsNumeric.Location = new Point(156, 316);
        timeoutSecondsNumeric.Maximum = new decimal(new int[] { 86400, 0, 0, 0 });
        timeoutSecondsNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        timeoutSecondsNumeric.Name = "timeoutSecondsNumeric";
        timeoutSecondsNumeric.Size = new Size(90, 23);
        timeoutSecondsNumeric.TabIndex = 16;
        timeoutSecondsNumeric.Value = new decimal(new int[] { 120, 0, 0, 0 });
        // 
        // targetLabel
        // 
        targetLabel.Location = new Point(16, 352);
        targetLabel.Name = "targetLabel";
        targetLabel.Size = new Size(458, 38);
        targetLabel.TabIndex = 17;
        targetLabel.Text = "監控：所選 Schema 的 VM_RESULT_CONTROL\n依 MAX(TIMETAG) 判斷新資料，唯讀查詢";
        // 
        // notificationGroup
        // 
        notificationGroup.Controls.Add(autoAlertCheckBox);
        notificationGroup.Controls.Add(alertTopicTextBoxLabel);
        notificationGroup.Controls.Add(alertTopicTextBox);
        notificationGroup.Controls.Add(alertPayloadTextBoxLabel);
        notificationGroup.Controls.Add(alertPayloadTextBox);
        notificationGroup.Controls.Add(pushAlertButton);
        notificationGroup.Controls.Add(recoveryEnabled);
        notificationGroup.Controls.Add(recoveryTopicLabel);
        notificationGroup.Controls.Add(recoveryTopic);
        notificationGroup.Controls.Add(recoveryPayloadLabel);
        notificationGroup.Controls.Add(recoveryPayload);
        notificationGroup.Controls.Add(pushRecoveryButton);
        notificationGroup.Controls.Add(variablesLabel);
        notificationGroup.Location = new Point(504, 0);
        notificationGroup.Name = "notificationGroup";
        notificationGroup.Size = new Size(542, 400);
        notificationGroup.TabIndex = 1;
        notificationGroup.TabStop = false;
        notificationGroup.Text = "警報與恢復通知";
        // 
        // autoAlertCheckBox
        // 
        autoAlertCheckBox.Checked = true;
        autoAlertCheckBox.CheckState = CheckState.Checked;
        autoAlertCheckBox.Location = new Point(154, 22);
        autoAlertCheckBox.Name = "autoAlertCheckBox";
        autoAlertCheckBox.Size = new Size(300, 27);
        autoAlertCheckBox.TabIndex = 0;
        autoAlertCheckBox.Text = "啟用逾時警報";
        // 
        // alertTopicTextBoxLabel
        // 
        alertTopicTextBoxLabel.Location = new Point(10, 54);
        alertTopicTextBoxLabel.Name = "alertTopicTextBoxLabel";
        alertTopicTextBoxLabel.Size = new Size(140, 27);
        alertTopicTextBoxLabel.TabIndex = 1;
        alertTopicTextBoxLabel.Text = "警報 Topic";
        alertTopicTextBoxLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // alertTopicTextBox
        // 
        alertTopicTextBox.Location = new Point(154, 56);
        alertTopicTextBox.Name = "alertTopicTextBox";
        alertTopicTextBox.Size = new Size(260, 23);
        alertTopicTextBox.TabIndex = 2;
        alertTopicTextBox.Text = "ncku/watchdog/alert";
        // 
        // pushAlertButton
        // 
        pushAlertButton.Enabled = false;
        pushAlertButton.Location = new Point(420, 54);
        pushAlertButton.Name = "pushAlertButton";
        pushAlertButton.Size = new Size(104, 27);
        pushAlertButton.TabIndex = 3;
        pushAlertButton.Text = "手動推送警報";
        pushAlertButton.Click += PushAlertButton_Click;
        // 
        // alertPayloadTextBoxLabel
        // 
        alertPayloadTextBoxLabel.Location = new Point(10, 96);
        alertPayloadTextBoxLabel.Name = "alertPayloadTextBoxLabel";
        alertPayloadTextBoxLabel.Size = new Size(140, 27);
        alertPayloadTextBoxLabel.TabIndex = 3;
        alertPayloadTextBoxLabel.Text = "警報內容（文字／JSON）";
        alertPayloadTextBoxLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // alertPayloadTextBox
        // 
        alertPayloadTextBox.Location = new Point(154, 90);
        alertPayloadTextBox.Multiline = true;
        alertPayloadTextBox.Name = "alertPayloadTextBox";
        alertPayloadTextBox.ScrollBars = ScrollBars.Vertical;
        alertPayloadTextBox.Size = new Size(370, 78);
        alertPayloadTextBox.TabIndex = 4;
        alertPayloadTextBox.Text = "{\"status\":\"no_data\",\"timestamp\":\"{timestamp}\",\"seconds\":{seconds},\"last_data\":\"{lastData}\"}";
        // 
        // recoveryEnabled
        // 
        recoveryEnabled.Checked = true;
        recoveryEnabled.CheckState = CheckState.Checked;
        recoveryEnabled.Location = new Point(154, 180);
        recoveryEnabled.Name = "recoveryEnabled";
        recoveryEnabled.Size = new Size(300, 27);
        recoveryEnabled.TabIndex = 5;
        recoveryEnabled.Text = "啟用資料恢復通知";
        // 
        // recoveryTopicLabel
        // 
        recoveryTopicLabel.Location = new Point(10, 212);
        recoveryTopicLabel.Name = "recoveryTopicLabel";
        recoveryTopicLabel.Size = new Size(140, 27);
        recoveryTopicLabel.TabIndex = 6;
        recoveryTopicLabel.Text = "恢復 Topic";
        recoveryTopicLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // recoveryTopic
        // 
        recoveryTopic.Location = new Point(154, 214);
        recoveryTopic.Name = "recoveryTopic";
        recoveryTopic.Size = new Size(260, 23);
        recoveryTopic.TabIndex = 7;
        recoveryTopic.Text = "ncku/watchdog/recovery";
        // 
        // pushRecoveryButton
        // 
        pushRecoveryButton.Enabled = false;
        pushRecoveryButton.Location = new Point(420, 212);
        pushRecoveryButton.Name = "pushRecoveryButton";
        pushRecoveryButton.Size = new Size(104, 27);
        pushRecoveryButton.TabIndex = 9;
        pushRecoveryButton.Text = "手動推送恢復";
        pushRecoveryButton.Click += PushRecoveryButton_Click;
        // 
        // recoveryPayloadLabel
        // 
        recoveryPayloadLabel.Location = new Point(10, 254);
        recoveryPayloadLabel.Name = "recoveryPayloadLabel";
        recoveryPayloadLabel.Size = new Size(140, 27);
        recoveryPayloadLabel.TabIndex = 8;
        recoveryPayloadLabel.Text = "恢復內容（文字／JSON）";
        recoveryPayloadLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // recoveryPayload
        // 
        recoveryPayload.Location = new Point(154, 248);
        recoveryPayload.Multiline = true;
        recoveryPayload.Name = "recoveryPayload";
        recoveryPayload.ScrollBars = ScrollBars.Vertical;
        recoveryPayload.Size = new Size(370, 78);
        recoveryPayload.TabIndex = 9;
        recoveryPayload.Text = "{\"status\":\"recovered\",\"timestamp\":\"{timestamp}\",\"last_data\":\"{lastData}\"}";
        // 
        // variablesLabel
        // 
        variablesLabel.Location = new Point(14, 334);
        variablesLabel.Name = "variablesLabel";
        variablesLabel.Size = new Size(510, 54);
        variablesLabel.TabIndex = 10;
        variablesLabel.Text = "可用變數：{timestamp}、{seconds}、{lastData}、{lastReceived}\n第一次成功查詢起算；執行中設定鎖定，停止後可修改。";
        // 
        // logTextBox
        // 
        logTextBox.BackColor = Color.FromArgb(25, 25, 25);
        logTextBox.Dock = DockStyle.Fill;
        logTextBox.Font = new Font("Consolas", 10F);
        logTextBox.ForeColor = Color.Gainsboro;
        logTextBox.Location = new Point(3, 562);
        logTextBox.Multiline = true;
        logTextBox.Name = "logTextBox";
        logTextBox.ReadOnly = true;
        logTextBox.ScrollBars = ScrollBars.Both;
        logTextBox.Size = new Size(1094, 214);
        logTextBox.TabIndex = 1;
        logTextBox.WordWrap = false;
        // 
        // manualPanel
        // 
        manualPanel.Controls.Add(clientIdTextBox);
        manualPanel.Controls.Add(topicTextBox);
        manualPanel.Controls.Add(publishTopicTextBox);
        manualPanel.Controls.Add(publishPayloadTextBox);
        manualPanel.Controls.Add(qosComboBox);
        manualPanel.Controls.Add(retainCheckBox);
        manualPanel.Controls.Add(validateJsonCheckBox);
        manualPanel.Controls.Add(publishButton);
        manualPanel.Location = new Point(0, 0);
        manualPanel.Name = "manualPanel";
        manualPanel.Size = new Size(700, 400);
        manualPanel.TabIndex = 1;
        manualPanel.Visible = false;
        // 
        // clientIdTextBox
        // 
        clientIdTextBox.Location = new Point(0, 0);
        clientIdTextBox.Name = "clientIdTextBox";
        clientIdTextBox.Size = new Size(260, 23);
        clientIdTextBox.TabIndex = 0;
        // 
        // topicTextBox
        // 
        topicTextBox.Location = new Point(0, 0);
        topicTextBox.Name = "topicTextBox";
        topicTextBox.Size = new Size(260, 23);
        topicTextBox.TabIndex = 1;
        // 
        // publishTopicTextBox
        // 
        publishTopicTextBox.Location = new Point(0, 0);
        publishTopicTextBox.Name = "publishTopicTextBox";
        publishTopicTextBox.Size = new Size(260, 23);
        publishTopicTextBox.TabIndex = 2;
        publishTopicTextBox.Text = "zhongli/zone1/compressor/c1/telemetry";
        // 
        // publishPayloadTextBox
        // 
        publishPayloadTextBox.Location = new Point(0, 0);
        publishPayloadTextBox.Name = "publishPayloadTextBox";
        publishPayloadTextBox.Size = new Size(260, 23);
        publishPayloadTextBox.TabIndex = 3;
        publishPayloadTextBox.Text = "{\r\n  \"compressor_drive_type\": \"VSD\",\r\n  \"timestamp\": \"2026-06-01T15:46:00+08:00\"\r\n}";
        // 
        // qosComboBox
        // 
        qosComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        qosComboBox.Items.AddRange(new object[] { "0", "1", "2" });
        qosComboBox.Location = new Point(0, 0);
        qosComboBox.Name = "qosComboBox";
        qosComboBox.Size = new Size(80, 23);
        qosComboBox.TabIndex = 4;
        // 
        // retainCheckBox
        // 
        retainCheckBox.Location = new Point(0, 0);
        retainCheckBox.Name = "retainCheckBox";
        retainCheckBox.Size = new Size(80, 23);
        retainCheckBox.TabIndex = 5;
        retainCheckBox.Text = "Retain";
        // 
        // validateJsonCheckBox
        // 
        validateJsonCheckBox.Checked = true;
        validateJsonCheckBox.CheckState = CheckState.Checked;
        validateJsonCheckBox.Location = new Point(0, 0);
        validateJsonCheckBox.Name = "validateJsonCheckBox";
        validateJsonCheckBox.Size = new Size(160, 23);
        validateJsonCheckBox.TabIndex = 6;
        validateJsonCheckBox.Text = "發布前驗證 JSON";
        // 
        // publishButton
        // 
        publishButton.Enabled = false;
        publishButton.Location = new Point(0, 0);
        publishButton.Name = "publishButton";
        publishButton.Size = new Size(100, 30);
        publishButton.TabIndex = 7;
        publishButton.Text = "發布";
        publishButton.Click += publishButton_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1100, 779);
        Controls.Add(shell);
        Controls.Add(manualPanel);
        MinimumSize = new Size(1100, 700);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "TESC WatchDog - CDB 監控 / 東元 MQTT";
        shell.ResumeLayout(false);
        shell.PerformLayout();
        tabs.ResumeLayout(false);
        dbPage.ResumeLayout(false);
        toolbar.ResumeLayout(false);
        toolbar.PerformLayout();
        monitorSettings.ResumeLayout(false);
        cdbGroup.ResumeLayout(false);
        cdbGroup.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pollSeconds).EndInit();
        ((System.ComponentModel.ISupportInitialize)timeoutSecondsNumeric).EndInit();
        notificationGroup.ResumeLayout(false);
        notificationGroup.PerformLayout();
        manualPanel.ResumeLayout(false);
        manualPanel.PerformLayout();
        ResumeLayout(false);
    }
    #endregion
}
