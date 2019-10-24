namespace DockerGui.Repositories
{
    public enum SentryEnum
    {
        Log,
        Stats
    }

    public enum SentryStatsGapEnum
    {
        Second = 1,
        ThreeSeconds = 3,
        TenSeconds = 10,
        ThirtySeconds = 30,
        Minute = 60,
        ThreeMinute = 180,
        TenMinute = 600,
        ThirtyMinute = 1800,
        Hour = 3600,
        ThreeHour = 10800,
    }
}
