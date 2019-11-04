using DockerGui.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace DockerGui.EfCore
{
    public interface IMySqlContext : IDbContextDependencies
    {
        int SaveChanges();

        DbSet<StatsEntity> StatsEntity { get; set; }
    }
}