using FIS.Contracts.Auth;
using FIS.Contracts.Clientes;
using Refit;
using ApiResponse = FIS.Contracts.Common.ApiResponse<object>;
using TokenApiResponse = FIS.Contracts.Common.ApiResponse<FIS.Contracts.Auth.TokenResponse>;
using ClientesApiResponse = FIS.Contracts.Common.ApiResponse<FIS.Contracts.Common.PagedResult<FIS.Contracts.Clientes.ClienteDto>>;
using StringApiResponse = FIS.Contracts.Common.ApiResponse<string>;

namespace FIS.Desktop.Services;

/// <summary>
/// Cliente HTTP tipado de la API mediante Refit.
/// Cada método mapea 1:1 a un endpoint REST del backend.
/// </summary>
public interface IFisApiClient
{
    [Post("/api/v1/auth/login")]
    Task<TokenApiResponse> Login([Body] LoginRequest req);

    [Get("/api/v1/clientes")]
    Task<ClientesApiResponse> ListarClientes(
        [Query] string? filtro,
        [Query] int page = 1,
        [Query] int pageSize = 25);

    [Get("/api/v1/clientes/admin-only")]
    Task<StringApiResponse> SoloAdmin();
}
