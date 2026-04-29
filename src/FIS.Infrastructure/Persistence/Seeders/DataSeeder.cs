using FIS.Application.Common.Interfaces;
using FIS.Domain.Entities;
using FIS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FIS.Infrastructure.Persistence.Seeders;

/// <summary>
/// Sembrado inicial de la base de datos: roles canónicos + usuario admin demo.
/// Idempotente: se puede ejecutar múltiples veces sin duplicar datos.
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(FisDbContext db, IPasswordHasher hasher)
    {
        await db.Database.MigrateAsync();
        await SembrarRolesAsync(db);
        await SembrarUsuarioAdminAsync(db, hasher);
        await db.SaveChangesAsync();
    }

    private static async Task SembrarRolesAsync(FisDbContext db)
    {
        var existentes = await db.Roles.Select(r => r.NombreRol).ToListAsync();
        var requeridos = new[]
        {
            Roles.Administrador, Roles.Cajero, Roles.Tecnico, Roles.Cliente
        };

        foreach (var nombre in requeridos.Where(n => !existentes.Contains(n)))
            db.Roles.Add(new Rol(nombre));

        await db.SaveChangesAsync();
    }

    private static async Task SembrarUsuarioAdminAsync(
        FisDbContext db, IPasswordHasher hasher)
    {
        const string adminUser = "admin";
        if (await db.Usuarios.AnyAsync(u => u.Username == adminUser))
            return;

        var rolAdmin = await db.Roles.FirstAsync(r => r.NombreRol == Roles.Administrador);

        var usuario = new Usuario(
            idRol: rolAdmin.IdRol,
            username: adminUser,
            passwordHash: hasher.Hash("Admin123*"),
            email: "admin@fullinternetservices.bo",
            nombres: "Administrador",
            apellidos: "Del Sistema");

        db.Usuarios.Add(usuario);
    }
}
