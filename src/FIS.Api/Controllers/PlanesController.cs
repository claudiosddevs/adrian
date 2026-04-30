using Asp.Versioning;
using FIS.Application.Planes.Commands;
using FIS.Application.Planes.Queries;
using FIS.Contracts.Common;
using FIS.Contracts.Planes;
using FIS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIS.Api.Controllers;

/// <summary>Gestión de Planes de Servicio (RF03, RF04).</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/planes")]
[Authorize]
public class PlanesController : ControllerBase
{
    private readonly IMediator _mediator;
    public PlanesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PlanDto>>), 200)]
    public async Task<IActionResult> Listar([FromQuery] bool soloActivos = true)
    {
        var result = await _mediator.Send(new ListarPlanesQuery(soloActivos));
        return Ok(ApiResponse<IReadOnlyList<PlanDto>>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ApiResponse<PlanDto>), 201)]
    public async Task<IActionResult> Crear([FromBody] CrearPlanRequest request)
    {
        var result = await _mediator.Send(new CrearPlanCommand(request));
        return StatusCode(201, ApiResponse<PlanDto>.Ok(result, "Plan creado."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ApiResponse<PlanDto>), 200)]
    public async Task<IActionResult> Actualizar(int id, [FromBody] CrearPlanRequest request)
    {
        var result = await _mediator.Send(new ActualizarPlanCommand(id, request));
        return Ok(ApiResponse<PlanDto>.Ok(result, "Plan actualizado."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Desactivar(int id)
    {
        await _mediator.Send(new DesactivarPlanCommand(id));
        return NoContent();
    }
}
