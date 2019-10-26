using System;
using System.Collections.Generic;
using DockerGui.Cores.Sentries.Models;

namespace DockerGui.Controllers.Sentries.Dtos
{
    public class SentryStatsDto
    {
        public DateTime Time { get; set; }

        public ulong Pids { get; set; }

        public decimal CpuPercent { get; set; }

        public decimal MemoryPercent { get; set; }

        public UnitValueDto MemoryValue { get; set; }

        public UnitValueDto MemoryLimit { get; set; }

        public IDictionary<string, ReadWriteDto> Nets { get; set; }

        public ReadWriteDto Block { get; set; }

        public class ReadWriteDto
        {
            public UnitValueDto Read { get; set; }

            public UnitValueDto Write { get; set; }
        }

        public class UnitValueDto
        {
            public string Unit { get; set; }
            public decimal Value { get; set; }
        }
    }
}