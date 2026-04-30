using FIS.Application.Common.Interfaces;
using FIS.Domain.Entities;
using FIS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FIS.Infrastructure.Persistence.Seeders;

/// <summary>
/// Datos de demostración para todos los RF01-RF17.
/// Idempotente: verifica antes de insertar cada entidad.
/// Ejecutar solo en entorno Development.
/// </summary>
public static class DemoDataSeeder
{
    public static async Task SeedDemoAsync(FisDbContext db, IPasswordHasher hasher)
    {
        // Asegurar que la migración esté aplicada primero
        await db.Database.MigrateAsync();

        // 1. Roles
        await SeedRolesAsync(db);

        // 2. Usuarios (RF16)
        var usuarios = await SeedUsuariosAsync(db, hasher);

        // 3. Clientes (RF02)
        var clientes = await SeedClientesAsync(db);

        // 4. Planes (RF03, RF04)
        var planes = await SeedPlanesAsync(db);

        // 5. Contratos (RF05)
        var contratos = await SeedContratosAsync(db, clientes, planes, usuarios);

        // 6. Pagos (RF06, RF07)
        await SeedPagosAsync(db, contratos, usuarios);

        // 7. Reclamos (RF09-RF12)
        await SeedReclamosAsync(db, clientes, contratos, usuarios);

        await db.SaveChangesAsync();
    }

    // ── 1. Roles ────────────────────────────────────────────────────────────
    private static async Task SeedRolesAsync(FisDbContext db)
    {
        var existentes = await db.Roles.Select(r => r.NombreRol).ToListAsync();
        var requeridos = new[] { Roles.Administrador, Roles.Cajero, Roles.Tecnico, Roles.Cliente };
        foreach (var nombre in requeridos.Where(n => !existentes.Contains(n)))
            db.Roles.Add(new Rol(nombre));
        await db.SaveChangesAsync();
    }

    // ── 2. Usuarios ─────────────────────────────────────────────────────────
    private static async Task<Dictionary<string, Usuario>> SeedUsuariosAsync(
        FisDbContext db, IPasswordHasher hasher)
    {
        var roles = await db.Roles.ToDictionaryAsync(r => r.NombreRol);
        var result = new Dictionary<string, Usuario>();

        async Task<Usuario> Ensure(string username, string nombres, string apellidos, string email, string pass, string rol)
        {
            var u = await db.Usuarios.Include(x => x.Rol).FirstOrDefaultAsync(x => x.Username == username);
            if (u is null)
            {
                u = new Usuario((byte)roles[rol].IdRol, username, hasher.Hash(pass), email, nombres, apellidos);
                db.Usuarios.Add(u);
                await db.SaveChangesAsync();
            }
            result[username] = u;
            return u;
        }

        await Ensure("admin", "Carlos", "Mendoza", "admin@fis.bo", "Admin123*", Roles.Administrador);
        await Ensure("cajero1", "María", "López", "cajero1@fis.bo", "Cajero123*", Roles.Cajero);
        await Ensure("cajero2", "Roberto", "Silva", "cajero2@fis.bo", "Cajero123*", Roles.Cajero);
        await Ensure("tecnico1", "Juan", "Pérez", "tecnico1@fis.bo", "Tecnico123*", Roles.Tecnico);
        await Ensure("tecnico2", "Ana", "García", "tecnico2@fis.bo", "Tecnico123*", Roles.Tecnico);
        await Ensure("tecnico3", "Luis", "Torrez", "tecnico3@fis.bo", "Tecnico123*", Roles.Tecnico);

        return result;
    }

    // ── 3. Clientes ─────────────────────────────────────────────────────────
    private static async Task<List<Cliente>> SeedClientesAsync(FisDbContext db)
    {
        if (await db.Clientes.AnyAsync()) return await db.Clientes.ToListAsync();

        var clientes = new List<Cliente>
        {
            new("N", "CLI-00001", "Pedro Antonio Rojas Vargas", "1234567", "pedro.rojas@gmail.com", "77712345", "Av. Heroínas 123", "Cochabamba"),
            new("N", "CLI-00002", "Luisa Fernanda Quispe Mamani", "2345678", "lquispe@hotmail.com", "77823456", "Calle Batallón 456", "Cochabamba"),
            new("N", "CLI-00003", "Miguel Angel Torres Cruz", "3456789", "mtorres@yahoo.com", "77934567", "Calle Potosí 789", "Oruro"),
            new("N", "CLI-00004", "Carmen Rosa Flores Condori", "4567890", "cflores@gmail.com", "77645678", "Av. Blanco Galindo km5", "Cochabamba"),
            new("J", "CLI-00005", "Industrias Textiles del Sur SRL", "4891234567", "info@textilesdelsur.bo", "44512345", "Zona Industrial Norte, Lote 12", "Cochabamba"),
            new("J", "CLI-00006", "Constructora Andina S.A.", "6012345678", "admin@constructoraandina.bo", "44623456", "Av. América Este 500", "Santa Cruz"),
            new("N", "CLI-00007", "Ricardo David Montaño Vaca", "5678901", "rmontano@gmail.com", "77756789", "Villa Adela, Mz 15 Lt 8", "La Paz"),
            new("N", "CLI-00008", "Silvia Elena Mamani Ticona", "6789012", "smamani@hotmail.com", "77867890", "El Alto, Zona 16 de Julio", "El Alto"),
            new("J", "CLI-00009", "Supermercados Amigo S.R.L.", "7023456789", "contacto@supramigo.bo", "44734567", "Av. Costanera 2200", "Trinidad"),
            new("N", "CLI-00010", "Óscar Gerardo Choque Lima", "7890123", "ochoque@gmail.com", "77978901", "Av. Santa Cruz 1500", "Sucre"),
            new("N", "CLI-00011", "Marcela Patricia Vásquez Heredia", "8901234", "mvasquez@gmail.com", "77589012", "Calle Colón 320", "Potosí"),
            new("J", "CLI-00012", "Farmacia Salud Total Ltda.", "9134567890", "gerencia@saludtotal.bo", "44845678", "Calle Aroma 760", "Cochabamba"),
        };

        db.Clientes.AddRange(clientes);
        await db.SaveChangesAsync();
        return clientes;
    }

    // ── 4. Planes ────────────────────────────────────────────────────────────
    private static async Task<List<PlanServicio>> SeedPlanesAsync(FisDbContext db)
    {
        if (await db.Planes.AnyAsync()) return await db.Planes.ToListAsync();

        var planes = new List<PlanServicio>
        {
            new("Plan Básico 10 Mbps", "Internet", 10m, 5m, 120.00m),
            new("Plan Estándar 25 Mbps", "Internet", 25m, 10m, 180.00m),
            new("Plan Premium 50 Mbps", "Internet", 50m, 25m, 280.00m),
            new("Plan Empresarial 100 Mbps", "Internet", 100m, 50m, 450.00m),
            new("Plan Fibra 200 Mbps", "Fibra", 200m, 100m, 680.00m),
            new("Hosting Básico 5 GB", "Hosting", null, null, 95.00m),
            new("Hosting Profesional 20 GB", "Hosting", null, null, 195.00m),
            new("Registro Dominio .bo", "Dominio", null, null, 350.00m),
        };

        db.Planes.AddRange(planes);
        await db.SaveChangesAsync();
        return planes;
    }

    // ── 5. Contratos ─────────────────────────────────────────────────────────
    private static async Task<List<Contrato>> SeedContratosAsync(
        FisDbContext db, List<Cliente> clientes, List<PlanServicio> planes, Dictionary<string, Usuario> usuarios)
    {
        if (await db.Contratos.AnyAsync()) return await db.Contratos.Include(c => c.Plan).ToListAsync();

        var adminId = usuarios["admin"].IdUsuario;
        var hoy = DateTime.UtcNow.Date;

        var contratos = new List<Contrato>
        {
            new(clientes[0].IdCliente, planes[0].IdPlan, adminId, "CTR-2025-0001", hoy.AddMonths(-14), hoy.AddMonths(-2)),
            new(clientes[1].IdCliente, planes[1].IdPlan, adminId, "CTR-2025-0002", hoy.AddMonths(-10), hoy.AddMonths(2)),
            new(clientes[2].IdCliente, planes[2].IdPlan, adminId, "CTR-2025-0003", hoy.AddMonths(-8), hoy.AddMonths(4)),
            new(clientes[3].IdCliente, planes[3].IdPlan, adminId, "CTR-2025-0004", hoy.AddMonths(-6), hoy.AddMonths(6)),
            new(clientes[4].IdCliente, planes[4].IdPlan, adminId, "CTR-2025-0005", hoy.AddMonths(-5), hoy.AddMonths(7)),
            new(clientes[5].IdCliente, planes[5].IdPlan, adminId, "CTR-2025-0006", hoy.AddMonths(-4), hoy.AddMonths(8)),
            new(clientes[6].IdCliente, planes[1].IdPlan, adminId, "CTR-2026-0001", hoy.AddMonths(-3), hoy.AddMonths(9)),
            new(clientes[7].IdCliente, planes[0].IdPlan, adminId, "CTR-2026-0002", hoy.AddMonths(-2), hoy.AddMonths(10)),
            new(clientes[8].IdCliente, planes[3].IdPlan, adminId, "CTR-2026-0003", hoy.AddMonths(-1), hoy.AddMonths(11)),
            new(clientes[9].IdCliente, planes[2].IdPlan, adminId, "CTR-2026-0004", hoy.AddDays(-15), hoy.AddDays(365)),
            new(clientes[10].IdCliente, planes[6].IdPlan, adminId, "CTR-2026-0005", hoy.AddDays(-5), hoy.AddDays(365)),
            new(clientes[11].IdCliente, planes[7].IdPlan, adminId, "CTR-2026-0006", hoy, hoy.AddDays(365)),
        };

        // Vencer el primero para simular mora
        contratos[0].Finalizar();

        db.Contratos.AddRange(contratos);
        await db.SaveChangesAsync();
        return contratos;
    }

    // ── 6. Pagos ─────────────────────────────────────────────────────────────
    private static async Task SeedPagosAsync(
        FisDbContext db, List<Contrato> contratos, Dictionary<string, Usuario> usuarios)
    {
        if (await db.Pagos.AnyAsync()) return;

        var cajero1Id = usuarios["cajero1"].IdUsuario;
        var cajero2Id = usuarios["cajero2"].IdUsuario;
        var hoy = DateTime.UtcNow;

        var pagos = new List<Pago>();

        // Contrato 1 — pagos históricos normales
        for (int m = 14; m >= 3; m--)
        {
            pagos.Add(new Pago(contratos[0].IdContrato, cajero1Id, "Efectivo",
                $"REC-2025-{pagos.Count + 1:D6}", (byte)((hoy.Month - m + 12) % 12 + 1),
                (short)(hoy.Year - (m > hoy.Month ? 1 : 0)), 120.00m, 0m, null));
        }

        // Contrato 2 — pagos con mora
        pagos.Add(new Pago(contratos[1].IdContrato, cajero1Id, "Transferencia",
            "REC-2025-000015", (byte)hoy.Month, (short)hoy.Year, 180.00m, 18.00m, null));
        pagos.Add(new Pago(contratos[1].IdContrato, cajero2Id, "QR",
            "REC-2025-000016", (byte)(hoy.Month == 1 ? 12 : hoy.Month - 1),
            (short)(hoy.Month == 1 ? hoy.Year - 1 : hoy.Year), 180.00m, 0m, null));

        // Contrato 3
        pagos.Add(new Pago(contratos[2].IdContrato, cajero2Id, "Efectivo",
            "REC-2026-000001", (byte)hoy.Month, (short)hoy.Year, 280.00m, 0m, null));

        // Pago anulado (RF07)
        var pagoAnular = new Pago(contratos[3].IdContrato, cajero1Id, "Tarjeta",
            "REC-2026-000002", (byte)hoy.Month, (short)hoy.Year, 450.00m, 0m, null);
        pagoAnular.Anular("Pago duplicado por error del operador.");
        pagos.Add(pagoAnular);

        // Pagos recientes contratos 4-8
        foreach (var (idx, cajeroId) in new[] { (3, cajero2Id), (4, cajero1Id), (5, cajero2Id), (6, cajero1Id), (7, cajero2Id) })
        {
            var plan = await db.Planes.FindAsync(contratos[idx].IdPlan);
            if (plan is null) continue;
            pagos.Add(new Pago(contratos[idx].IdContrato, cajeroId, "QR",
                $"REC-2026-{pagos.Count + 1:D6}", (byte)hoy.Month, (short)hoy.Year, plan.PrecioMensual, 0m, null));
        }

        db.Pagos.AddRange(pagos);
        await db.SaveChangesAsync();
    }

    // ── 7. Reclamos ───────────────────────────────────────────────────────────
    private static async Task SeedReclamosAsync(
        FisDbContext db, List<Cliente> clientes, List<Contrato> contratos, Dictionary<string, Usuario> usuarios)
    {
        if (await db.Reclamos.AnyAsync()) return;

        var tecnico1Id = usuarios["tecnico1"].IdUsuario;
        var tecnico2Id = usuarios["tecnico2"].IdUsuario;
        var tecnico3Id = usuarios["tecnico3"].IdUsuario;
        var adminId = usuarios["admin"].IdUsuario;

        var reclamos = new List<Reclamo>
        {
            // RF09: recepcionado, sin técnico asignado (RF10: Leve)
            new(clientes[0].IdCliente, contratos[0].IdContrato, adminId,
                "REC-202604-0001", TipoReclamo.Leve, "Velocidad lenta a partir de las 20:00 hrs.", 2, CanalEntrada.Telefono),

            // RF11: técnico asignado, en proceso (RF10: Medio)
            new(clientes[1].IdCliente, contratos[1].IdContrato, adminId,
                "REC-202604-0002", TipoReclamo.Medio, "Corte intermitente durante el trabajo remoto. Afecta videollamadas.", 2, CanalEntrada.Telefono),

            // RF12: estado "Observado" (RF10: Complejo)
            new(clientes[2].IdCliente, contratos[2].IdContrato, adminId,
                "REC-202604-0003", TipoReclamo.Complejo, "Sin servicio desde el día 28. Router parpadea en rojo.", 1, CanalEntrada.Telefono),

            // Cerrado con solución (RF09-RF12)
            new(clientes[3].IdCliente, contratos[3].IdContrato, adminId,
                "REC-202603-0001", TipoReclamo.Leve, "No puede acceder a ciertas páginas web.", 3, CanalEntrada.Web),

            // Otro cerrado
            new(clientes[4].IdCliente, contratos[4].IdContrato, adminId,
                "REC-202603-0002", TipoReclamo.Medio, "Velocidad de subida muy baja, afecta backups a la nube.", 2, CanalEntrada.Telefono),

            // Reclamo nuevo sin asignar
            new(clientes[5].IdCliente, null, adminId,
                "REC-202604-0004", TipoReclamo.Leve, "Solicita información sobre upgrade de plan.", 4, CanalEntrada.Presencial),

            // Complejo en proceso
            new(clientes[6].IdCliente, contratos[6].IdContrato, adminId,
                "REC-202604-0005", TipoReclamo.Complejo, "No hay conectividad en toda la cuadra. Posible corte de fibra.", 1, CanalEntrada.Telefono),

            // Medio observado
            new(clientes[7].IdCliente, contratos[7].IdContrato, adminId,
                "REC-202604-0006", TipoReclamo.Medio, "Dirección MAC del router cambió, no puede autenticarse.", 2, CanalEntrada.Telefono),
        };

        db.Reclamos.AddRange(reclamos);
        await db.SaveChangesAsync();

        // Asignar técnicos y cambiar estados (RF11, RF12)
        reclamos[1].AsignarTecnico(tecnico1Id, 0);
        reclamos[2].AsignarTecnico(tecnico2Id, 0);
        reclamos[2].CambiarEstado(EstadoReclamo.Observado);
        reclamos[3].AsignarTecnico(tecnico1Id, 1);
        reclamos[3].RegistrarSolucion("Se configuraron los servidores DNS alternativos. Problema resuelto.");
        reclamos[3].Calificar(5, 45);
        reclamos[4].AsignarTecnico(tecnico2Id, 1);
        reclamos[4].RegistrarSolucion("Se limitó la banda ancha de subida contratada y se ajustó la QoS del router.");
        reclamos[4].Calificar(4, 90);
        reclamos[6].AsignarTecnico(tecnico3Id, 0);
        reclamos[7].AsignarTecnico(tecnico1Id, 2);
        reclamos[7].CambiarEstado(EstadoReclamo.Observado);

        await db.SaveChangesAsync();
    }
}
