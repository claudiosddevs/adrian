using FIS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIS.Infrastructure.Persistence.Configurations;

public class PlanServicioConfiguration : IEntityTypeConfiguration<PlanServicio>
{
    public void Configure(EntityTypeBuilder<PlanServicio> b)
    {
        b.ToTable("PLAN_SERVICIO", t => t.HasCheckConstraint(
            "CK_PLAN_tipo_servicio",
            "tipo_servicio IN ('Internet Residencial','Internet Empresarial'," +
            "'Hosting Web','Dominio .bo','Correo Corporativo')"));
        b.HasKey(x => x.IdPlan);

        b.Property(x => x.IdPlan).HasColumnName("id_plan").ValueGeneratedOnAdd();
        b.Property(x => x.NombrePlan).HasColumnName("nombre_plan").HasMaxLength(100).IsRequired();
        b.Property(x => x.TipoServicio).HasColumnName("tipo_servicio").HasMaxLength(30).IsRequired();
        b.Property(x => x.VelocidadBajadaMbps).HasColumnName("velocidad_bajada_mbps").HasColumnType("decimal(8,2)");
        b.Property(x => x.VelocidadSubidaMbps).HasColumnName("velocidad_subida_mbps").HasColumnType("decimal(8,2)");
        b.Property(x => x.PrecioMensual).HasColumnName("precio_mensual").HasColumnType("decimal(10,2)").IsRequired();
        b.Property(x => x.Activo).HasColumnName("activo").HasDefaultValue(true);
    }
}
