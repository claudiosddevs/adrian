using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FIS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<Domain.Services.CalculadoraMoraService>();

        return services;
    }
}
