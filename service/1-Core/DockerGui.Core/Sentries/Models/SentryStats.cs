using System;
using System.Collections.Generic;
using System.Linq;
using Docker.DotNet.Models;
using Newtonsoft.Json;

namespace DockerGui.Core.Sentries.Models
{
    public partial class SentryStats
    {
        public SentryStats() { }
        public SentryStats(ContainerStatsResponse response)
        {
            ContainerId = response.ID;
            Time = response.Read;
            Pids = response.PidsStats.Current;
            // cpu
            if (response.CPUStats.SystemUsage - response.PreCPUStats.SystemUsage > 0
                && response.CPUStats.CPUUsage.TotalUsage - response.PreCPUStats.CPUUsage.TotalUsage >= 0)
            {
                CpuPercent = (((decimal)(response.CPUStats.CPUUsage.TotalUsage - response.PreCPUStats.CPUUsage.TotalUsage) /
                             (decimal)(response.CPUStats.SystemUsage - response.PreCPUStats.SystemUsage)) * 100M).ToFixed(2);
            }

            // 缓存
            var cache = 0UL;
            if (response.MemoryStats.Stats.TryGetValue("cache", out var c))
            {
                cache = c;
            }
            var memoryByte = response.MemoryStats.Usage - cache;
            MemoryValue = ByteUnitConvert(memoryByte);
            MemoryPercent = ((decimal)memoryByte / (decimal)response.MemoryStats.MaxUsage * 100M).ToFixed(2);
            MemoryLimit = ByteUnitConvert(response.MemoryStats.Limit);
            // net
            Nets = response.Networks?.ToDictionary(
                x => x.Key,
                x =>
                new SentryStatsReadWrite
                {
                    Read = ByteUnitConvert(x.Value.RxBytes, 1000, 1),
                    Write = ByteUnitConvert(x.Value.TxBytes, 1000, 1)
                }
            );
            // block
            Block = new SentryStatsReadWrite
            {
                Read = ByteUnitConvert(response.BlkioStats.IoServiceBytesRecursive
                            .Where(x => x.Op == "Read")
                            .Sum(x => (decimal)x.Value), 1000, 1),
                Write = ByteUnitConvert(response.BlkioStats.IoServiceBytesRecursive
                            .Where(x => x.Op == "Write")
                            .Sum(x => (decimal)x.Value), 1000, 1)
            };
        }

        [JsonProperty("cid")]
        public string ContainerId { get; set; }

        [JsonProperty("t")]
        public DateTime Time { get; set; }

        [JsonProperty("p")]
        public ulong Pids { get; set; }

        [JsonProperty("c")]
        public decimal CpuPercent { get; set; }

        [JsonProperty("mp")]
        public decimal MemoryPercent { get; set; }

        [JsonProperty("mv")]
        public SentryStatsUnitValue MemoryValue { get; set; }

        [JsonProperty("ml")]
        public SentryStatsUnitValue MemoryLimit { get; set; }

        [JsonProperty("n")]
        public IDictionary<string, SentryStatsReadWrite> Nets { get; set; }

        [JsonProperty("b")]
        public SentryStatsReadWrite Block { get; set; }

        private SentryStatsUnitValue ByteUnitConvert(decimal number, int unit = 1024, int digit = 2)
        {
            return new SentryStatsUnitValue(unit, digit, number);
        }
    }
}