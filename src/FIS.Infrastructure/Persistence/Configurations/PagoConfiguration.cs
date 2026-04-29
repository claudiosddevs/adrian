using FIS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIS.Infrastructure.Persistence.Configurations;

public class PagoConfiguration : IEntityTypeConfiguration<Pago>
{
    public void Configure(EntityTypeBuilder<Pago> b)
    {
        b.ToTable("PAGO", t =>
        {
            t.HasCheckConstraint("CK_PAGO_metodo",
                "metodo_pago IN ('Efectivo','QR','Transferencia','Tarjeta Débito','Tarjeta Crédito')");
            t.HasCheckConstraint("CK_PAGO_periodo_mes", "periodo_mes BETWEEN 1 AND 12");
        });
        b.HasKey(x => x.IdPago);

        b.Property(x => x.IdPago).HasColumnName("id_pago").ValueGeneratedOnAdd();
        b.Property(x => x.IdContrato).HasColumnName("id_contrato").IsRequired();
        b.Property(x => x.IdCajero).HasColumnName("id_cajero").IsRequired();
        b.Property(x => x.MetodoPago).HasColumnName("metodo_pago").HasMaxLength(20).IsRequired();
        b.Property(x => x.NumeroRecibo).HasColumnName("numero_recibo").HasMaxLength(30).IsRequired();
        b.Property(x => x.PeriodoMes).HasColumnName("periodo_mes").IsRequired();
        b.Property(x => x.PeriodoAnio).HasColumnName("periodo_anio").IsRequired();
        b.Property(x => x.FechaPago).HasColumnName("fecha_pago").HasDefaultValueSql("GETDATE()");
        b.Property(x => x.MontoTotal).HasColumnName("monto_total").HasColumnType("decimal(10,2)").IsRequired();
        b.Property(x => x.MontoMora).HasColumnName("monto_mora").HasColumnType("decimal(10,2)").HasDefaultValue(0m);
        b.Property(x => x.Conceptos).HasColumnName("conceptos").HasColumnType("nvarchar(max)");
        b.Property(x => x.Anulado).HasColumnName("anulado").HasDefaultValue(false);
        b.Property(x => x.MotivoAnulacion).HasColumnName("motivo_anulacion").HasMaxLength(500);
        b.Property(x => x.FechaAnulacion).HasColumnName("fecha_anulacion");

        b.HasIndex(x => x.NumeroRecibo).IsUnique();

        b.HasOne(x => x.Contrato)
            .WithMany(c => c.Pagos)
            .HasForeignKey(x => x.IdContrato)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Cajero)
            .WithMany()
            .HasForeignKey(x => x.IdCajero)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
