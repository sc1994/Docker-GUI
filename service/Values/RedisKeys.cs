namespace DockerGui.Values
{
    public class RedisKeys
    {
        /// <summary>
        /// 哨兵列表数据
        /// </summary>
        public static string SentryList(SentryEnum sentry, string id) => $"list:sentry:{id}:{sentry.GetHashCode()}";

        /// <summary>
        /// 扩展SentryList到具体统计粒度
        /// </summary>
        /// <param name="key"></param>
        /// <param name="grading"></param>
        /// <returns></returns>
        public static string SentryStatsList(SentryEnum sentry, string id, SentryStatsGapEnum grading) => $"{SentryList(sentry, id)}:{grading.GetHashCode()}";
    }
}