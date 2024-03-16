using Microsoft.EntityFrameworkCore;
using SharedModels;

namespace LoggingApp.Infrastructure.Data
{
    public class LogsDbContext : DbContext
    {
        public LogsDbContext(DbContextOptions<LogsDbContext> options) 
            : base(options) 
        {
        }

        public DbSet<ActionLogDto> Actions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActionLogDto>()
                .HasKey(a => a.DateTime);
            base.OnModelCreating(modelBuilder);
        }
    }
}
