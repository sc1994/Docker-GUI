
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using service.Entities;

namespace service.Repositories
{
    public class MySql : DbContext, IMySql
    {
        private readonly IConfiguration _config;
        public MySql(IConfiguration config)
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