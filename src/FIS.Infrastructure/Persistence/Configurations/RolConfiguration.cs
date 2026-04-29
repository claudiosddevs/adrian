using FIS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIS.Infrastructure.Persistence.Configurations;

public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> b)
    {
        b.ToTable("ROL");
        b.HasKey(x => x.IdRol);

        b.Property(x => x.IdRol).HasColumnName("id_rol").ValueGeneratedOnAdd();
        b.Property(x => x.NombreRol).HasColumnName("nombre_rol").HasMaxLength(20).IsRequired();
        b.Property(x => x.Activo).HasColumnName("activo").HasDefaultValue(true);

        b.HasIndex(x => x.NombreRol).IsUnique();
    }
}
