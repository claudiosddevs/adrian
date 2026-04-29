using FIS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIS.Infrastructure.Persistence.Configurations;

public class ReclamoConfiguration : IEntityTypeConfiguration<Reclamo>
{
    public void Configure(EntityTypeBuilder<Reclamo> b)
    {
        b.ToTable("RECLAMO", t =>
        {
            t.HasCheckConstraint("CK_RECLAMO_tipo",
                "tipo_reclamo IN ('Leve','Medio','Complejo')");
            t.HasCheckConstraint("CK_RECLAMO_estado",
                "estado IN ('Recepcionado','En Proceso','Observado','Cerrado')");
            t.HasCheckConstraint("CK_RECLAMO_prioridad", "prioridad BETWEEN 1 AND 5");
            t.HasCheckConstraint("CK_RECLAMO_canal",
                "canal_entrada IN ('telefono','web','presencial','app')");
            t.HasCheckConstraint("CK_RECLAMO_calificacion",
                "calificacion IS NULL OR (calificacion BETWEEN 1 AND 5)");
        });
        b.HasKey(x => x.IdReclamo);

        b.Property(x => x.IdReclamo).HasColumnName("id_reclamo").ValueGeneratedOnAdd();
        b.Property(x => x.IdCliente).HasColumnName("id_cliente").IsRequired();
        b.Property(x => x.IdContrato).HasColumnName("id_contrato");
        b.Property(x => x.IdTecnico).HasColumnName("id_tecnico");
        b.Property(x => x.IdUsuarioRegistro).HasColumnName("id_usuario_registro").IsRequired();
        b.Property(x => x.NumeroReclamo).HasColumnName("numero_reclamo").HasMaxLength(30).IsRequired();
        b.Property(x => x.TipoReclamo).HasColumnName("tipo_reclamo").HasMaxLength(10).IsRequired();
        b.Property(x => x.Estado).HasColumnName("estado").HasMaxLength(15)
            .HasDefaultValue("Recepcionado").IsRequired();
        b.Property(x => x.DescripcionProblema).HasColumnName("descripcion_problema")
            .HasColumnType("nvarchar(max)").IsRequired();
        b.Property(x => x.SolucionAplicada).HasColumnName("solucion_aplicada")
            .HasColumnType("nvarchar(max)");
        b.Property(x => x.Prioridad).HasColumnName("prioridad").HasDefaultValue((byte)3);
        b.Property(x => x.CanalEntrada).HasColumnName("canal_entrada").HasMaxLength(10)
            .HasDefaultValue("telefono").IsRequired();
        b.Property(x => x.FechaApertura).HasColumnName("fecha_apertura").HasDefaultValueSql("GETDATE()");
        b.Property(x => x.FechaCierre).HasColumnName("fecha_cierre");
        b.Property(x => x.Calificacion).HasColumnName("calificacion");
        b.Property(x => x.TiempoRespuestaMin).HasColumnName("tiempo_respuesta_min");
        b.Property(x => x.RutaAudio).HasColumnName("ruta_audio").HasMaxLength(500);
        b.Property(x => x.DuracionAudioSeg).HasColumnName("duracion_audio_seg");
        b.Property(x => x.HashSha256).HasColumnName("hash_sha256").HasMaxLength(64);

        b.HasIndex(x => x.NumeroReclamo).IsUnique();

        b.HasOne(x => x.Cliente)
            .WithMany(c => c.Reclamos)
            .HasForeignKey(x => x.IdCliente)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Contrato)
            .WithMany()
            .HasForeignKey(x => x.IdContrato)
            .OnDelete(DeleteBehavior.NoAction);

        b.HasOne(x => x.Tecnico)
            .WithMany()
            .HasForeignKey(x => x.IdTecnico)
            .OnDelete(DeleteBehavior.NoAction);

        b.HasOne(x => x.UsuarioRegistro)
            .WithMany()
            .HasForeignKey(x => x.IdUsuarioRegistro)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
