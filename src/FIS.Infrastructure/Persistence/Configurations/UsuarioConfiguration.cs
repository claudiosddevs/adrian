using FIS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIS.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> b)
    {
        b.ToTable("USUARIO");
        b.HasKey(x => x.IdUsuario);

        b.Property(x => x.IdUsuario).HasColumnName("id_usuario").ValueGeneratedOnAdd();
        b.Property(x => x.IdRol).HasColumnName("id_rol").IsRequired();
        b.Property(x => x.Username).HasColumnName("username").HasMaxLength(50).IsRequired();
        b.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
        b.Property(x => x.Email).HasColumnName("email").HasMaxLength(150).IsRequired();
        b.Property(x => x.Nombres).HasColumnName("nombres").HasMaxLength(100).IsRequired();
        b.Property(x => x.Apellidos).HasColumnName("apellidos").HasMaxLength(100).IsRequired();
        b.Property(x => x.Especialidad).HasColumnName("especialidad").HasMaxLength(100);
        b.Property(x => x.Activo).HasColumnName("activo").HasDefaultValue(true);
        b.Property(x => x.IntentosFallidos).HasColumnName("intentos_fallidos").HasDefaultValue((byte)0);
        b.Property(x => x.BloqueadoHasta).HasColumnName("bloqueado_hasta");
        b.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("GETDATE()");

        b.HasIndex(x => x.Username).IsUnique();
        b.HasIndex(x => x.Email).IsUnique().HasDatabaseName("IX_USUARIO_email");

        b.HasOne(x => x.Rol)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(x => x.IdRol)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
