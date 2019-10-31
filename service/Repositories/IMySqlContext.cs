using DockerGui.Entities;
using Microsoft.EntityFrameworkCore;

namespace DockerGui.Repositories
{
    public interface IMySqlContext
    {
        int SaveChanges();
        DbSet<StatsEntity> StatsEntity { get; set; }
    }
}