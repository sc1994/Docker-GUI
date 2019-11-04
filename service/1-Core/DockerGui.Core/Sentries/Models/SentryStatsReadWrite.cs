using Newtonsoft.Json;

namespace DockerGui.Core.Sentries.Models
{
    public class SentryStatsReadWrite
    {
        [JsonProperty("r")]
        public SentryStatsUnitValue Read { get; set; }

        [JsonProperty("w")]
        public SentryStatsUnitValue Write { get; set; }
    }
}