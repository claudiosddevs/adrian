using Asp.Versioning;
using FIS.Application.Clientes.Queries;
using FIS.Contracts.Clientes;
using FIS.Contracts.Common;
using FIS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIS.Api.Controllers;

/// <summary>
/// Endpoint protegido. Demuestra el RBAC: solo Administrador o Cajero
/// pueden listar clientes.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/clientes")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Lista paginada de clientes activos.</summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.Administrador},{Roles.Cajero}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ClienteDto>>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Listar(
        [FromQuery] string? filtro,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25)
    {
        var result = await _mediator.Send(new ListarClientesQuery(filtro, page, pageSize));
        return Ok(ApiResponse<PagedResult<ClienteDto>>.Ok(result));
    }

    /// <summary>Endpoint de prueba que requiere rol Administrador.</summary>
    [HttpGet("admin-only")]
    [Authorize(Roles = Roles.Administrador)]
    public IActionResult SoloAdmin() =>
        Ok(ApiResponse<string>.Ok("Acceso concedido al administrador.",
            "Endpoint de demostración RBAC"));
}
