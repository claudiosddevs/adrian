using FIS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIS.Infrastructure.Persistence.Configurations;

public class MoraConfiguration : IEntityTypeConfiguration<Mora>
{
    public void Configure(EntityTypeBuilder<Mora> b)
    {
        b.ToTable("MORA");
        b.HasKey(x => x.IdMora);

        b.Property(x => x.IdMora).HasColumnName("id_mora").ValueGeneratedOnAdd();
        b.Property(x => x.IdContrato).HasColumnName("id_contrato").IsRequired();
        b.Property(x => x.EnMora).HasColumnName("en_mora").HasDefaultValue(false);
        b.Property(x => x.FechaInicioMora).HasColumnName("fecha_inicio_mora").HasColumnType("date");
        b.Property(x => x.MontoAdeudado).HasColumnName("monto_adeudado")
            .HasColumnType("decimal(10,2)").HasDefaultValue(0m);
        b.Property(x => x.ServicioCortado).HasColumnName("servicio_cortado").HasDefaultValue(false);
        b.Property(x => x.FechaRegularizacion).HasColumnName("fecha_regularizacion").HasColumnType("date");
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("GETDATE()");

        b.HasIndex(x => x.IdContrato).IsUnique();

        b.HasOne(x => x.Contrato)
            .WithOne(c => c.Mora!)
            .HasForeignKey<Mora>(x => x.IdContrato)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
