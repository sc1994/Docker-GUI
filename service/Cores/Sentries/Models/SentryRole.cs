using System.Collections.Generic;
using DockerGui.Repositories;
using Newtonsoft.Json;

namespace DockerGui.Cores.Sentries.Models
{
    public class SentryRole
    {
        private const int Hour = 3600;
        private const int Day = 86400;

        public SentryRole(SentryStatsGapEnum secondGap)
        {
            SecondGap = secondGap;
            MaxLimit = Day * secondGap.GetHashCode();
            UseLimit = Hour * secondGap.GetHashCode() / 8; // 减少redis压力
        }

        /// <summary>
        /// 时间粒度
        /// </summary>
        /// <value></value>
        public SentryStatsGapEnum SecondGap { get; set; }

        /// <summary>
        /// 限制进入reids数据的最大条数
        /// </summary>
        /// <value></value>
        public int MaxLimit { get; set; }

        /// <summary>
        /// 限制时间范围<=当前值使用,使用当前粒度
        /// </summary>
        /// <value></value>
        public int UseLimit { get; set; }

        /// <summary>
        /// 临时存储数据,达到聚合要求时,聚合进入redis
        /// </summary>
        /// <value></value>
        [JsonIgnore]
        public List<SentryStats> TempList { get; set; }
    }
}