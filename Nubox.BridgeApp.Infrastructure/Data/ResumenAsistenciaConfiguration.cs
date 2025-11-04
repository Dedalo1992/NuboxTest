using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nubox.BridgeApp.Domain.Entities;

namespace Nubox.BridgeApp.Infrastructure.Data
{
    public class ResumenAsistenciaConfiguration : IEntityTypeConfiguration<ResumenAsistencia>
    {
        public void Configure(EntityTypeBuilder<ResumenAsistencia> builder)
        {
            builder.HasKey(ra => ra.Id);
            builder.Property(ra => ra.EmpresaId).IsRequired().HasMaxLength(50);
            builder.Property(ra => ra.TrabajadorId).IsRequired().HasMaxLength(50);
            builder.Property(ra => ra.PeriodoClave).IsRequired().HasMaxLength(20);
            builder.Property(ra => ra.Rut).HasMaxLength(15);

            builder.HasIndex(ra => new { ra.EmpresaId, ra.TrabajadorId, ra.PeriodoClave }).IsUnique();
        }
    }
}
