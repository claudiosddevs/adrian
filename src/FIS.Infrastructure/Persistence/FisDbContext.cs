using FIS.Domain.Entities;
using FIS.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FIS.Infrastructure.Persistence;

/// <summary>
/// DbContext principal. Mapea las 8 tablas optimizadas definidas en db.sql:
/// ROL, USUARIO, CLIENTE, PLAN_SERVICIO, CONTRATO, PAGO, MORA, RECLAMO.
/// </summary>
public class FisDbContext : DbContext, IUnitOfWork
{
    public FisDbContext(DbContextOptions<FisDbContext> options) : base(options) { }

    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<PlanServicio> Planes => Set<PlanServicio>();
    public DbSet<Contrato> Contratos => Set<Contrato>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<Mora> Moras => Set<Mora>();
    public DbSet<Reclamo> Reclamos => Set<Reclamo>();
    public DbSet<BitacoraOperacion> Bitacora => Set<BitacoraOperacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("dbo");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FisDbContext).Assembly);
    }
}
