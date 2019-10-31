using System.ComponentModel.DataAnnotations;
using DockerGui.Service.Cores.Sentries.Models;

namespace DockerGui.Service.Entities
{
    public class StatsEntity : Entity
    {
        public StatsEntity() { }

        public StatsEntity(SentryStats stats)
        {
            Created = stats.Time.ToTimeStampMinutes();
            OnlyKey = Created + "_" + stats.ContainerId;
            JsonContent = stats.Serialize();
        }

        [Required]
        public string OnlyKey { get; set; }

        [Required]
        public string JsonContent { get; set; }
    }
}