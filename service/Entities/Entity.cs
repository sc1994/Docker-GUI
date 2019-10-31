using System.ComponentModel.DataAnnotations;

namespace DockerGui.Entities
{
    public class Entity
    {
        [Key]
        public long Id { get; set; }

        public long Created { get; set; }
    }
}