using DockerGui.Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DockerGui.Service.Repositories
{
    public class MySqlContext : DbContext, IMySqlContext
    {
        private readonly IConfiguration _config;
        public MySqlContext(IConfiguration config)
        {
            _config = config;
        }

        public DbSet<StatsEntity> StatsEntity { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_config.GetConnectionString("MySql"));
        }
    }
}