using TESCWatchDog;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        int checks = 0;
        void Check(bool condition, string name)
        {
            if (!condition) throw new Exception(name);
            checks++; Console.WriteLine("PASS " + name);
        }
        var old = new DateTime(2020, 1, 1);
        var interval = TimeSpan.FromSeconds(10);
        Check(PollTiming.NextDelay(interval, TimeSpan.FromSeconds(3), true) == interval, "Fast successful query gets full countdown");
        Check(PollTiming.NextDelay(interval, TimeSpan.FromSeconds(10), true) == interval, "Work reaching interval gets full cooldown");
        Check(PollTiming.NextDelay(interval, TimeSpan.FromSeconds(15), true) == interval, "Slow query does not trigger catch-up polling");
        Check(PollTiming.NextDelay(interval, TimeSpan.FromSeconds(1), false) == interval, "Fast failure waits full interval");
        Check(PollTiming.NextDelay(interval, TimeSpan.FromSeconds(30), false) == interval, "Timeout waits full interval");
        var state = new DatabaseWatchState();
        string? Observe(int seconds, DateTime? value) => state.Observe(value, TimeSpan.FromSeconds(seconds), TimeSpan.FromSeconds(120));
        Check(Observe(0, old) == null, "Old baseline does not alarm immediately");
        Check(Observe(119, old) == null, "No alarm before 120 seconds");
        Check(Observe(120, old) == "alert", "Alarm at 120 seconds");
        Check(Observe(130, old) == null, "No duplicate alarm");
        Check(Observe(140, old.AddSeconds(40)) == "recovery", "One recovery on fresh data");
        Check(Observe(150, old.AddSeconds(40)) == null, "No duplicate recovery");
        Check(Observe(250, old.AddSeconds(80)) == null, "New data resets timeout");
        Check(Observe(369, old.AddSeconds(80)) == null, "Reset grants full timeout");
        Check(Observe(370, old.AddSeconds(80)) == "alert", "Second incident alarms");
        Check(Observe(380, old) == null && state.InAlarm, "Backward TIMETAG is not recovery");
        var empty = new DatabaseWatchState();
        Check(empty.Observe(null, TimeSpan.Zero, TimeSpan.FromSeconds(120)) == null, "Empty baseline valid");
        Check(empty.Observe(null, TimeSpan.FromSeconds(120), TimeSpan.FromSeconds(120)) == "alert", "Empty table timeout");
        Check(empty.Observe(old, TimeSpan.FromSeconds(130), TimeSpan.FromSeconds(120)) == "recovery", "First row recovers empty table");
        ApplicationConfiguration.Initialize();
        var ini = CdbIniSettings.Parse("[CDB]\nDSN=\"DB18SQL\"\nIP=\"localhost\"\nDatabase=\"test\"\nUID=user\nPWD=\"a=b;# test\"\nTableUserName=dbo\nType=SQLServer");
        Check(ini.Server == "localhost" && ini.Database == "test", "INI quoted server and database");
        Check(ini.Password == "a=b;# test", "INI password preserves equals and comment characters");
        Check(!ini.IntegratedSecurity && !ini.TrustServerCertificate, "INI secure defaults");
        Check(ini.QueryTimeoutSeconds == 10, "Existing INI remains compatible with timeout default");
        Check(CdbIniSettings.Parse("[CDB]\nIP=localhost\nDatabase=test\nQueryTimeoutSeconds=45").QueryTimeoutSeconds == 45, "INI independent query timeout");
        bool timeoutRejected = false;
        try { CdbIniSettings.Parse("[CDB]\nIP=localhost\nDatabase=test\nQueryTimeoutSeconds=0"); } catch (FormatException) { timeoutRejected = true; }
        Check(timeoutRejected, "INI rejects unlimited query timeout");
        bool rejected = false;
        try { CdbIniSettings.Parse("[CDB]\nIP=localhost\nDSN=name"); } catch (FormatException) { rejected = true; }
        Check(rejected, "DSN cannot silently replace database");
        using var form = new Form1();
        var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
        var receiptType = typeof(Form1).GetNestedType("Receipt", System.Reflection.BindingFlags.NonPublic)!;
        var receiptSignal = new TaskCompletionSource<bool>();
        var receipt = Activator.CreateInstance(receiptType, "ncku/watchdog/alert", System.Text.Encoding.UTF8.GetBytes("probe"), receiptSignal)!;
        var pending = (System.Collections.IList)typeof(Form1).GetField("receipts", flags)!.GetValue(form)!;
        pending.Add(receipt);
        var match = typeof(Form1).GetMethod("MatchReceipt", flags)!;
        void Receive(string topic, string payload, bool retained = false) => match.Invoke(form, new object[] {
            new MQTTnet.MqttApplicationMessageBuilder().WithTopic(topic).WithPayload(payload).WithRetainFlag(retained).Build() });
        Receive("ncku/watchdog/alert", "probe", true);
        Check(!receiptSignal.Task.IsCompleted, "Retained snapshot cannot confirm publication");
        Receive("ncku/watchdog/recovery", "probe");
        Receive("ncku/watchdog/alert", "different");
        Check(!receiptSignal.Task.IsCompleted, "Receipt requires exact topic and payload");
        Receive("ncku/watchdog/alert", "probe");
        Check(receiptSignal.Task.Result && pending.Count == 0, "Matching receipt completes and removes pending confirmation");
        var disconnectedSignal = new TaskCompletionSource<bool>();
        pending.Add(Activator.CreateInstance(receiptType, "ncku/watchdog/alert", Array.Empty<byte>(), disconnectedSignal)!);
        typeof(Form1).GetMethod("DisconnectReceipts", flags)!.Invoke(form, null);
        Check(!disconnectedSignal.Task.Result && pending.Count == 0, "Disconnect marks pending receipt unconfirmed");
        form.StartPosition = FormStartPosition.Manual;
        form.Location = new Point(-32000, -32000);
        form.ShowInTaskbar = false;
        form.Show();
        Application.DoEvents();
        form.PerformLayout();
        using var bitmap = new Bitmap(form.Width, form.Height);
        form.DrawToBitmap(bitmap, form.ClientRectangle);
        var path = Path.GetFullPath("output/watchdog-cdb-preview.png");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        bitmap.Save(path);
        var shell = form.Controls.OfType<TableLayoutPanel>().Single();
        var tabs = shell.Controls.OfType<TabControl>().Single();
        Check(tabs.TabPages.Count == 1, "Manual MQTT page is hidden");
        form.Close();
        Check(true, "Form construction and offscreen render (no network)");
        Console.WriteLine($"{checks} checks passed. Preview: {path}");
    }
}
