using FIS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIS.Infrastructure.Persistence.Configurations;

public class BitacoraConfiguration : IEntityTypeConfiguration<BitacoraOperacion>
{
    public void Configure(EntityTypeBuilder<BitacoraOperacion> b)
    {
        b.ToTable("BITACORA");
        b.HasKey(x => x.IdBitacora);
        b.Property(x => x.IdBitacora).HasColumnName("id_bitacora").ValueGeneratedOnAdd();
        b.Property(x => x.Tabla).HasColumnName("tabla").HasMaxLength(50).IsRequired();
        b.Property(x => x.Operacion).HasColumnName("operacion").HasMaxLength(10).IsRequired();
        b.Property(x => x.ValoresAnteriores).HasColumnName("valores_anteriores").HasColumnType("nvarchar(max)");
        b.Property(x => x.ValoresNuevos).HasColumnName("valores_nuevos").HasColumnType("nvarchar(max)");
        b.Property(x => x.UsuarioAccion).HasColumnName("usuario_accion").HasMaxLength(100);
        b.Property(x => x.FechaHora).HasColumnName("fecha_hora").HasDefaultValueSql("GETDATE()");
    }
}
