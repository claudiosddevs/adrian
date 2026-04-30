using FIS.Application.Common.Interfaces;
using FIS.Domain.Interfaces;
using FIS.Infrastructure.Identity;
using FIS.Infrastructure.Persistence;
using FIS.Infrastructure.Persistence.Repositories;
using FIS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FIS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("FisDatabase")
            ?? throw new InvalidOperationException(
                "Falta la cadena de conexión 'FisDatabase' en la configuración.");

        services.AddDbContext<FisDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(typeof(FisDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<FisDbContext>());

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<IContratoRepository, ContratoRepository>();
        services.AddScoped<IPagoRepository, PagoRepository>();
        services.AddScoped<IReclamoRepository, ReclamoRepository>();

        services.AddScoped<IReporteService, ReporteService>();

        services.Configure<JwtSettings>(config.GetSection("Jwt"));
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        return services;
    }
}
