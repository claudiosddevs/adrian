using FIS.Desktop.Forms.Auth;
using FIS.Desktop.Forms.Dashboard;
using FIS.Desktop.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace FIS.Desktop;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var services = new ServiceCollection();

        services.AddSingleton<TokenStore>();
        services.AddSingleton<SesionUsuario>();
        services.AddTransient<AuthHeaderHandler>();

        var apiBaseUrl = config["Api:BaseUrl"]
            ?? throw new InvalidOperationException("Falta Api:BaseUrl en appsettings.json");

        services.AddRefitClient<IFisApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl))
            .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddTransient<frmLogin>();
        services.AddTransient<frmDashboard>(provider =>
            new frmDashboard(
                provider.GetRequiredService<IFisApiClient>(),
                provider.GetRequiredService<TokenStore>(),
                provider.GetRequiredService<SesionUsuario>(),
                provider));

        var sp = services.BuildServiceProvider();

        Application.Run(sp.GetRequiredService<frmLogin>());
    }
}
