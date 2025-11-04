using Microsoft.EntityFrameworkCore;
using Nubox.BridgeApp.Domain.Entities;

namespace Nubox.BridgeApp.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }

        public DbSet<ResumenAsistencia> ResumenesAsistencia => Set<ResumenAsistencia>();
        public DbSet<Sincronizacion> Sincronizaciones => Set<Sincronizacion>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
