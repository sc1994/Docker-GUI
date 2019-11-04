using Newtonsoft.Json;

namespace DockerGui.Core.Sentries.Models
{
    public class SentryStatsUnitValue
    {
        public SentryStatsUnitValue() { }

        public SentryStatsUnitValue(int minUnit, int digit, decimal sourceValue)
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