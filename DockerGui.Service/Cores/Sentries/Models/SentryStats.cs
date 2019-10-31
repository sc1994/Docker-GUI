using System;
using System.Collections.Generic;
using System.Linq;
using Docker.DotNet.Models;
using Newtonsoft.Json;

namespace DockerGui.Service.Cores.Sentries.Models
{
    public class SentryStats
    {
        public SentryStats() { }
        public SentryStats(ContainerStatsResponse response)
        {
            ContainerId = response.ID;
            Time = response.Read;
            Pids = response.PidsStats.Current;
            // cpu
            CpuPercent = (((decimal)(response.CPUStats.CPUUsage.TotalUsage - response.PreCPUStats.CPUUsage.TotalUsage) /
                         (decimal)(response.CPUStats.SystemUsage - response.PreCPUStats.SystemUsage)) * 100M).ToFixed(2);
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
                new ReadWrite
                {
                    Read = ByteUnitConvert(x.Value.RxBytes, 1000, 1),
                    Write = ByteUnitConvert(x.Value.TxBytes, 1000, 1)
                }
            );
            // block
            Block = new ReadWrite
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
        public UnitValue MemoryValue { get; set; }

        [JsonProperty("ml")]
        public UnitValue MemoryLimit { get; set; }

        [JsonProperty("n")]
        public IDictionary<string, ReadWrite> Nets { get; set; }

        [JsonProperty("b")]
        public ReadWrite Block { get; set; }

        private UnitValue ByteUnitConvert(decimal number, int unit = 1024, int digit = 2)
        {
            return new UnitValue(unit, digit, number);
        }

        public class ReadWrite
        {
            [JsonProperty("r")]
            public UnitValue Read { get; set; }

            [JsonProperty("w")]
            public UnitValue Write { get; set; }
        }

        public class UnitValue
        {
            public UnitValue() { }

            public UnitValue(int minUnit, int digit, decimal sourceValue)
            {
                MinUnit = minUnit;
                Digit = digit;
                SourceValue = sourceValue;
            }

            [JsonProperty("m")]
            public int MinUnit { get; set; }

            [JsonProperty("d")]
            public int Digit { get; set; }

            [JsonProperty("s")]
            public decimal SourceValue { get; set; }

            [JsonIgnore]
            public string Unit => GetUnit();

            [JsonIgnore]
            public decimal Value => GetValue();

            private string GetUnit()
            {
                if (MinUnit == 0)
                    return "B";
                if (SourceValue / MinUnit < MinUnit)
                    return "KB";
                if (SourceValue / MinUnit / MinUnit < MinUnit)
                    return "MB";
                return "GB";
            }

            private decimal GetValue()
            {
                if (MinUnit == 0)
                    return 0;
                if (SourceValue / MinUnit < MinUnit)
                {
                    return (SourceValue / MinUnit).ToFixed(2);
                }
                else if (SourceValue / MinUnit / MinUnit < MinUnit)
                {
                    return (SourceValue / MinUnit / MinUnit).ToFixed(2);
                }
                else
                {
                    return (SourceValue / MinUnit / MinUnit / MinUnit).ToFixed(2);
                }
            }
        }
    }
}