using Asp.Versioning;
using FIS.Application.Clientes.Commands;
using FIS.Application.Clientes.Queries;
using FIS.Contracts.Clientes;
using FIS.Contracts.Common;
using FIS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIS.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/clientes")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IMediator _mediator;
    public ClientesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Lista paginada de clientes.</summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.Administrador},{Roles.Cajero}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ClienteDto>>), 200)]
    public async Task<IActionResult> Listar(
        [FromQuery] string? filtro, [FromQuery] int page = 1, [FromQuery] int pageSize = 25)
    {
        var result = await _mediator.Send(new ListarClientesQuery(filtro, page, pageSize));
        return Ok(ApiResponse<PagedResult<ClienteDto>>.Ok(result));
    }

    /// <summary>Obtiene un cliente por ID.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = $"{Roles.Administrador},{Roles.Cajero}")]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Obtener(int id)
    {
        var result = await _mediator.Send(new ObtenerClienteQuery(id));
        return Ok(ApiResponse<ClienteDto>.Ok(result));
    }

    /// <summary>Crea un nuevo cliente (RF02).</summary>
    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), 201)]
    public async Task<IActionResult> Crear([FromBody] CrearClienteRequest request)
    {
        var result = await _mediator.Send(new CrearClienteCommand(request));
        return CreatedAtAction(nameof(Obtener), new { id = result.IdCliente },
            ApiResponse<ClienteDto>.Ok(result, "Cliente creado exitosamente."));
    }

    /// <summary>Actualiza datos de un cliente (RF02).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), 200)]
    public async Task<IActionResult> Actualizar(int id, [FromBody] CrearClienteRequest request)
    {
        var result = await _mediator.Send(new ActualizarClienteCommand(id, request));
        return Ok(ApiResponse<ClienteDto>.Ok(result, "Cliente actualizado."));
    }

    /// <summary>Desactiva un cliente (RF02).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Desactivar(int id)
    {
        await _mediator.Send(new DesactivarClienteCommand(id));
        return NoContent();
    }
}
