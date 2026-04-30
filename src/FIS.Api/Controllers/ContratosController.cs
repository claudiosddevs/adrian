using Asp.Versioning;
using FIS.Application.Contratos.Commands;
using FIS.Application.Contratos.Queries;
using FIS.Contracts.Common;
using FIS.Contracts.Contratos;
using FIS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIS.Api.Controllers;

/// <summary>Gestión de Contratos (RF05).</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/contratos")]
[Authorize]
public class ContratosController : ControllerBase
{
    private readonly IMediator _mediator;
    public ContratosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Roles = $"{Roles.Administrador},{Roles.Cajero}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ContratoDto>>), 200)]
    public async Task<IActionResult> Listar([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new ListarContratosQuery(page, pageSize));
        return Ok(ApiResponse<PagedResult<ContratoDto>>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ApiResponse<ContratoDto>), 201)]
    public async Task<IActionResult> Registrar([FromBody] RegistrarContratoRequest request)
    {
        var result = await _mediator.Send(new RegistrarContratoCommand(request));
        return StatusCode(201, ApiResponse<ContratoDto>.Ok(result, "Contrato registrado."));
    }

    [HttpPatch("{id:int}/estado")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> CambiarEstado(int id, [FromQuery] string accion)
    {
        await _mediator.Send(new CambiarEstadoContratoCommand(id, accion));
        return NoContent();
    }
}
