using DockerGui.EfEntity;
using Microsoft.EntityFrameworkCore;

namespace DockerGui.EfCore
{
    public interface IMySqlContext
    {
        int SaveChanges();
        DbSet<StatsEntity> StatsEntity { get; set; }
    }
}