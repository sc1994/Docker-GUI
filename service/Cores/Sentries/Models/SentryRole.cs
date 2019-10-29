using System.Collections.Generic;
using DockerGui.Repositories;
using Newtonsoft.Json;

namespace DockerGui.Cores.Sentries.Models
{
    public class SentryRole
    {
        public SentryRole()
        {
            List = new List<SentryRoleItem>
            {
                new SentryRoleItem(SentryStatsGapEnum.ThreeSeconds),
                new SentryRoleItem(SentryStatsGapEnum.TenSeconds),
                new SentryRoleItem(SentryStatsGapEnum.ThirtySeconds),
                new SentryRoleItem(SentryStatsGapEnum.Minute),
                new SentryRoleItem(SentryStatsGapEnum.ThreeMinute),
                new SentryRoleItem(SentryStatsGapEnum.TenMinute),
                new SentryRoleItem(SentryStatsGapEnum.ThirtyMinute)
            };
        }

        public List<SentryRoleItem> List { get; }
    }
    public class SentryRoleItem
    {
        private const int Hour = 3600;

        public SentryRoleItem(SentryStatsGapEnum secondGap)
        {
            SecondGap = secondGap;
            UseLimit = Hour * secondGap.GetHashCode();
            TempList = new List<SentryStats>();
        }

        /// <summary>
        /// 时间粒度
        /// </summary>
        /// <value></value>
        public SentryStatsGapEnum SecondGap { get; set; }

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