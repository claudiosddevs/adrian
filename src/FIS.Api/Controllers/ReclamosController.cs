using Asp.Versioning;
using FIS.Application.Reclamos.Commands;
using FIS.Application.Reclamos.Queries;
using FIS.Contracts.Common;
using FIS.Contracts.Reclamos;
using FIS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIS.Api.Controllers;

/// <summary>Soporte técnico / Reclamos (RF09-RF13).</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reclamos")]
[Authorize]
public class ReclamosController : ControllerBase
{
    private readonly IMediator _mediator;
    public ReclamosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Roles = $"{Roles.Administrador},{Roles.Tecnico}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ReclamoDto>>), 200)]
    public async Task<IActionResult> Listar(
        [FromQuery] string? estado,
        [FromQuery] int? tecnicoId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new ListarReclamosQuery(estado, tecnicoId, page, pageSize));
        return Ok(ApiResponse<PagedResult<ReclamoDto>>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Administrador},{Roles.Tecnico}")]
    [ProducesResponseType(typeof(ApiResponse<ReclamoDto>), 201)]
    public async Task<IActionResult> Registrar([FromBody] RegistrarReclamoRequest request)
    {
        var result = await _mediator.Send(new RegistrarReclamoCommand(request));
        return StatusCode(201, ApiResponse<ReclamoDto>.Ok(result, "Reclamo registrado."));
    }

    [HttpPatch("{id:int}/tecnico")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ApiResponse<ReclamoDto>), 200)]
    public async Task<IActionResult> AsignarTecnico(int id, [FromBody] AsignarTecnicoRequest request)
    {
        var result = await _mediator.Send(new AsignarTecnicoCommand(id, request));
        return Ok(ApiResponse<ReclamoDto>.Ok(result, "Técnico asignado."));
    }

    [HttpPatch("{id:int}/estado")]
    [Authorize(Roles = $"{Roles.Administrador},{Roles.Tecnico}")]
    [ProducesResponseType(typeof(ApiResponse<ReclamoDto>), 200)]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoReclamoRequest request)
    {
        var result = await _mediator.Send(new CambiarEstadoReclamoCommand(id, request));
        return Ok(ApiResponse<ReclamoDto>.Ok(result, "Estado actualizado."));
    }
}
