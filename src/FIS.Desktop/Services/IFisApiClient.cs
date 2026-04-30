using FIS.Contracts.Auth;
using FIS.Contracts.Clientes;
using FIS.Contracts.Contratos;
using FIS.Contracts.Pagos;
using FIS.Contracts.Planes;
using FIS.Contracts.Reclamos;
using FIS.Contracts.Reportes;
using FIS.Contracts.Usuarios;
using Refit;

using TokenApiResponse = FIS.Contracts.Common.ApiResponse<FIS.Contracts.Auth.TokenResponse>;
using ClientesPagedResponse = FIS.Contracts.Common.ApiResponse<FIS.Contracts.Common.PagedResult<FIS.Contracts.Clientes.ClienteDto>>;
using ClienteApiResponse = FIS.Contracts.Common.ApiResponse<FIS.Contracts.Clientes.ClienteDto>;
using PlanesListResponse = FIS.Contracts.Common.ApiResponse<System.Collections.Generic.IReadOnlyList<FIS.Contracts.Planes.PlanDto>>;
using PlanApiResponse = FIS.Contracts.Common.ApiResponse<FIS.Contracts.Planes.PlanDto>;
using ContratosPagedResponse = FIS.Contracts.Common.ApiResponse<FIS.Contracts.Common.PagedResult<FIS.Contracts.Contratos.ContratoDto>>;
using ContratoApiResponse = FIS.Contracts.Common.ApiResponse<FIS.Contracts.Contratos.ContratoDto>;
using PagosPagedResponse = FIS.Contracts.Common.ApiResponse<FIS.Contracts.Common.PagedResult<FIS.Contracts.Pagos.PagoDto>>;
using PagoApiResponse = FIS.Contracts.Common.ApiResponse<FIS.Contracts.Pagos.PagoDto>;
using ReclamosPagedResponse = FIS.Contracts.Common.ApiResponse<FIS.Contracts.Common.PagedResult<FIS.Contracts.Reclamos.ReclamoDto>>;
using ReclamoApiResponse = FIS.Contracts.Common.ApiResponse<FIS.Contracts.Reclamos.ReclamoDto>;
using UsuariosListResponse = FIS.Contracts.Common.ApiResponse<System.Collections.Generic.IReadOnlyList<FIS.Contracts.Usuarios.UsuarioDto>>;
using UsuarioApiResponse = FIS.Contracts.Common.ApiResponse<FIS.Contracts.Usuarios.UsuarioDto>;
using ReporteMoraResponse = FIS.Contracts.Common.ApiResponse<System.Collections.Generic.IReadOnlyList<FIS.Contracts.Reportes.ReporteMoraDto>>;
using ReporteVentasResponse = FIS.Contracts.Common.ApiResponse<System.Collections.Generic.IReadOnlyList<FIS.Contracts.Reportes.ReporteVentasDto>>;
using ReporteTecnicosResponse = FIS.Contracts.Common.ApiResponse<System.Collections.Generic.IReadOnlyList<FIS.Contracts.Reportes.ReporteTecnicoDto>>;

namespace FIS.Desktop.Services;

public interface IFisApiClient
{
    // -- Auth --
    [Post("/api/v1/auth/login")]
    Task<TokenApiResponse> Login([Body] LoginRequest req);

    // -- Clientes (RF02) --
    [Get("/api/v1/clientes")]
    Task<ClientesPagedResponse> ListarClientes([Query] string? filtro, [Query] int page = 1, [Query] int pageSize = 25);

    [Get("/api/v1/clientes/{id}")]
    Task<ClienteApiResponse> ObtenerCliente(int id);

    [Post("/api/v1/clientes")]
    Task<ClienteApiResponse> CrearCliente([Body] CrearClienteRequest request);

    [Put("/api/v1/clientes/{id}")]
    Task<ClienteApiResponse> ActualizarCliente(int id, [Body] CrearClienteRequest request);

    [Delete("/api/v1/clientes/{id}")]
    Task DesactivarCliente(int id);

    // -- Planes (RF03, RF04) --
    [Get("/api/v1/planes")]
    Task<PlanesListResponse> ListarPlanes([Query] bool soloActivos = true);

    [Post("/api/v1/planes")]
    Task<PlanApiResponse> CrearPlan([Body] CrearPlanRequest request);

    [Put("/api/v1/planes/{id}")]
    Task<PlanApiResponse> ActualizarPlan(int id, [Body] CrearPlanRequest request);

    [Delete("/api/v1/planes/{id}")]
    Task DesactivarPlan(int id);

    // -- Contratos (RF05) --
    [Get("/api/v1/contratos")]
    Task<ContratosPagedResponse> ListarContratos([Query] int page = 1, [Query] int pageSize = 20);

    [Post("/api/v1/contratos")]
    Task<ContratoApiResponse> RegistrarContrato([Body] RegistrarContratoRequest request);

    [Patch("/api/v1/contratos/{id}/estado")]
    Task CambiarEstadoContrato(int id, [Query] string accion);

    // -- Pagos (RF06, RF07) --
    [Get("/api/v1/pagos")]
    Task<PagosPagedResponse> ListarPagos([Query] int page = 1, [Query] int pageSize = 20);

    [Post("/api/v1/pagos")]
    Task<PagoApiResponse> RegistrarPago([Body] RegistrarPagoRequest request);

    [Post("/api/v1/pagos/anular")]
    Task AnularPago([Body] AnularPagoRequest request);

    // -- Reclamos (RF09-RF13) --
    [Get("/api/v1/reclamos")]
    Task<ReclamosPagedResponse> ListarReclamos([Query] string? estado, [Query] int? tecnicoId, [Query] int page = 1, [Query] int pageSize = 20);

    [Post("/api/v1/reclamos")]
    Task<ReclamoApiResponse> RegistrarReclamo([Body] RegistrarReclamoRequest request);

    [Patch("/api/v1/reclamos/{id}/tecnico")]
    Task<ReclamoApiResponse> AsignarTecnico(int id, [Body] AsignarTecnicoRequest request);

    [Patch("/api/v1/reclamos/{id}/estado")]
    Task<ReclamoApiResponse> CambiarEstadoReclamo(int id, [Body] CambiarEstadoReclamoRequest request);

    // -- Usuarios (RF16) --
    [Get("/api/v1/usuarios")]
    Task<UsuariosListResponse> ListarUsuarios();

    [Post("/api/v1/usuarios")]
    Task<UsuarioApiResponse> CrearUsuario([Body] CrearUsuarioRequest request);

    // -- Reportes (RF15) --
    [Get("/api/v1/reportes/mora")]
    Task<ReporteMoraResponse> ReporteMora();

    [Get("/api/v1/reportes/ventas")]
    Task<ReporteVentasResponse> ReporteVentas([Query] int anio = 0);

    [Get("/api/v1/reportes/tecnicos")]
    Task<ReporteTecnicosResponse> ReporteTecnicos();
}
