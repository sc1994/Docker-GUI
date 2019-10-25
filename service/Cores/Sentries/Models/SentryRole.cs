using System.Collections.Generic;
using Newtonsoft.Json;

namespace DockerGui.Cores.Sentries.Models
{
    public class SentryRole
    {
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