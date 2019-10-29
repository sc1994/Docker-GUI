namespace DockerGui.Repositories
{
    public enum SentryEnum
    {
        Log,
        Stats
    }

    public enum SentryStatsGapEnum
    {
        ThreeSeconds = 3,
        TenSeconds = 10,
        ThirtySeconds = 30,
        Minute = 60,
        ThreeMinute = 180,
        TenMinute = 600,
        ThirtyMinute = 1800,
    }
}
