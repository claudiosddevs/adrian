using FIS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIS.Infrastructure.Persistence.Configurations;

public class ContratoConfiguration : IEntityTypeConfiguration<Contrato>
{
    public void Configure(EntityTypeBuilder<Contrato> b)
    {
        b.ToTable("CONTRATO", t =>
        {
            t.HasCheckConstraint("CK_CONTRATO_estado",
                "estado IN ('activo','suspendido','finalizado','cancelado')");
            t.HasCheckConstraint("CK_CONTRATO_fechas", "fecha_fin > fecha_inicio");
        });
        b.HasKey(x => x.IdContrato);

        b.Property(x => x.IdContrato).HasColumnName("id_contrato").ValueGeneratedOnAdd();
        b.Property(x => x.IdCliente).HasColumnName("id_cliente").IsRequired();
        b.Property(x => x.IdPlan).HasColumnName("id_plan").IsRequired();
        b.Property(x => x.IdUsuarioRegistro).HasColumnName("id_usuario_registro").IsRequired();
        b.Property(x => x.NumeroContrato).HasColumnName("numero_contrato").HasMaxLength(30).IsRequired();
        b.Property(x => x.FechaInicio).HasColumnName("fecha_inicio").HasColumnType("date").IsRequired();
        b.Property(x => x.FechaFin).HasColumnName("fecha_fin").HasColumnType("date").IsRequired();
        b.Property(x => x.Estado).HasColumnName("estado").HasMaxLength(20)
            .HasDefaultValue("activo").IsRequired();
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");

        b.HasIndex(x => x.NumeroContrato).IsUnique();
        b.HasIndex(x => x.IdCliente).HasDatabaseName("IX_CONTRATO_cliente");

        b.HasOne(x => x.Cliente)
            .WithMany(c => c.Contratos)
            .HasForeignKey(x => x.IdCliente)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Plan)
            .WithMany(p => p.Contratos)
            .HasForeignKey(x => x.IdPlan)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.UsuarioRegistro)
            .WithMany()
            .HasForeignKey(x => x.IdUsuarioRegistro)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
