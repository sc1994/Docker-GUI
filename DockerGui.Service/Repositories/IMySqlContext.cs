using DockerGui.Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace DockerGui.Service.Repositories
{
    public interface IMySqlContext
    {
        int SaveChanges();
        DbSet<StatsEntity> StatsEntity { get; set; }
    }
}