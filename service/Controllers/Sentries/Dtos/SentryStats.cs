using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Docker.DotNet.Models;
using service.Tools;

namespace service.Controllers.Sentries.Dtos
{
    public class SentryStats
    {
        public SentryStats() { }
        public SentryStats(ContainerStatsResponse response)
        {
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

        public DateTime Time { get; set; }

        public ulong Pids { get; set; }

        public decimal CpuPercent { get; set; }

        public decimal MemoryPercent { get; set; }

        public UnitValue MemoryValue { get; set; }

        public UnitValue MemoryLimit { get; set; }

        public IDictionary<string, ReadWrite> Nets { get; set; }

        public ReadWrite Block { get; set; }

        private UnitValue ByteUnitConvert(decimal number, int unit = 1024, int digit = 2)
        {
            return new UnitValue(unit, digit, number);
        }

        public class ReadWrite
        {
            public UnitValue Read { get; set; }

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

            public int MinUnit { get; set; }
            public int Digit { get; set; }
            public decimal SourceValue { get; set; }
            public string Unit => GetUnit();
            public decimal Value => GetValue();

            private string GetUnit()
            {
                if (SourceValue / MinUnit < MinUnit)
                {
                    return "KB";
                }
                else if (SourceValue / MinUnit / MinUnit < MinUnit)
                {
                    return "MB";
                }
                else
                {
                    return "GB";
                }
            }

            private decimal GetValue()
            {
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