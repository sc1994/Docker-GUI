using System.ComponentModel.DataAnnotations;

namespace DockerGui.Service.Entities
{
    public class Entity
    {
        [Key]
        public long Id { get; set; }

        public long Created { get; set; }
    }
}