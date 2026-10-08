namespace TESCWatchDog;

// 使用 Stopwatch 的單調時間，電腦校時與資料庫舊日期不影響兩分鐘計時。
public sealed class DatabaseWatchState
{
    public bool Initialized { get; private set; }
    public DateTime? Latest { get; private set; }
    public TimeSpan LastChange { get; private set; }
    public bool InAlarm { get; private set; }
    public string? Observe(DateTime? latest, TimeSpan now, TimeSpan timeout)
    {
        if (!Initialized)
        {
            // 第一筆成功查詢只建立基準；空表也從此刻开始計時。
            Initialized = true; Latest = latest; LastChange = now;
            return null;
        }
        if (latest.HasValue && (!Latest.HasValue || latest > Latest))
        {
            // TIMETAG 需向前增加；相同／回填的舊時間不視為新資料。
            Latest = latest; LastChange = now;
            var recovered = InAlarm;
            InAlarm = false;
            return recovered ? "recovery" : null;
        }
        if (!InAlarm && now - LastChange >= timeout)
        {
            InAlarm = true;
            return "alert";
        }
        return null;
    }
}
