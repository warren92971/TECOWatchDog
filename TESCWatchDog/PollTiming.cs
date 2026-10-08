namespace TESCWatchDog;

/// <summary>每輪工作完成後休息完整間隔；不重疊，也不補跑錯過的查詢。</summary>
public static class PollTiming
{
    public static TimeSpan NextDelay(TimeSpan interval, TimeSpan elapsed, bool succeeded)
    {
        if (interval <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(interval));
        if (elapsed < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(elapsed));
        // 使用者設定的是「查詢完成後到下一次查詢」的倒數時間。
        // 即使 SQL 很快完成，畫面仍會完整顯示 10、9、8……1。
        return interval;
    }
}
