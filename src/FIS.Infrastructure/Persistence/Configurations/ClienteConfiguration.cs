using FIS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIS.Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> b)
    {
        b.ToTable("CLIENTE", t =>
            t.HasCheckConstraint("CK_CLIENTE_tipo", "tipo_cliente IN ('N','J')"));
        b.HasKey(x => x.IdCliente);

        b.Property(x => x.IdCliente).HasColumnName("id_cliente").ValueGeneratedOnAdd();
        b.Property(x => x.TipoCliente).HasColumnName("tipo_cliente")
            .HasColumnType("char(1)").IsRequired();
        b.Property(x => x.CodigoCliente).HasColumnName("codigo_cliente")
            .HasMaxLength(20).IsRequired();
        b.Property(x => x.NombreRazonSocial).HasColumnName("nombre_razon_social")
            .HasMaxLength(200).IsRequired();
        b.Property(x => x.NitCi).HasColumnName("nit_ci").HasMaxLength(20).IsRequired();
        b.Property(x => x.Email).HasColumnName("email").HasMaxLength(150).IsRequired();
        b.Property(x => x.Telefono).HasColumnName("telefono").HasMaxLength(20).IsRequired();
        b.Property(x => x.Direccion).HasColumnName("direccion").HasMaxLength(500).IsRequired();
        b.Property(x => x.Ciudad).HasColumnName("ciudad").HasMaxLength(100).IsRequired();
        b.Property(x => x.Activo).HasColumnName("activo").HasDefaultValue(true);
        b.Property(x => x.FechaRegistro).HasColumnName("fecha_registro")
            .HasColumnType("date").HasDefaultValueSql("GETDATE()");

        b.HasIndex(x => x.CodigoCliente).IsUnique();
        b.HasIndex(x => x.NitCi).IsUnique().HasDatabaseName("IX_CLIENTE_nit");
        b.HasIndex(x => x.Email).IsUnique().HasDatabaseName("IX_CLIENTE_email");
        b.HasIndex(x => x.NombreRazonSocial).HasDatabaseName("IX_CLIENTE_nombre");
        b.HasIndex(x => x.Telefono).HasDatabaseName("IX_CLIENTE_telefono");
    }
}
