using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nubox.BridgeApp.Domain.Entities;

namespace Nubox.BridgeApp.Infrastructure.Data
{
    public class SincronizacionConfiguration : IEntityTypeConfiguration<Sincronizacion>
    {
        public void Configure(EntityTypeBuilder<Sincronizacion> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.EmpresaId).IsRequired().HasMaxLength(50);
            builder.Property(s => s.PeriodoClave).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Status).IsRequired();
            builder.Property(s => s.CorrelationId).IsRequired().HasMaxLength(100);

            builder.HasIndex(s => s.CorrelationId).IsUnique();
            builder.HasIndex(s => new { s.EmpresaId, s.PeriodoClave });
        }
    }
}
