using Asp.Versioning;
using FIS.Application.Reportes.Queries;
using FIS.Contracts.Common;
using FIS.Contracts.Reportes;
using FIS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIS.Api.Controllers;

/// <summary>Generación de reportes (RF15) y bitácora (RF17).</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reportes")]
[Authorize(Roles = Roles.Administrador)]
public class ReportesController : ControllerBase
{
    private readonly IMediator _mediator;
    public ReportesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Clientes con mora (RF08 + RF15).</summary>
    [HttpGet("mora")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ReporteMoraDto>>), 200)]
    public async Task<IActionResult> Mora()
    {
        var result = await _mediator.Send(new ReporteMoraQuery());
        return Ok(ApiResponse<IReadOnlyList<ReporteMoraDto>>.Ok(result));
    }

    /// <summary>Ventas por mes (RF15).</summary>
    [HttpGet("ventas")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ReporteVentasDto>>), 200)]
    public async Task<IActionResult> Ventas([FromQuery] int anio = 0)
    {
        if (anio == 0) anio = DateTime.UtcNow.Year;
        var result = await _mediator.Send(new ReporteVentasQuery(anio));
        return Ok(ApiResponse<IReadOnlyList<ReporteVentasDto>>.Ok(result));
    }

    /// <summary>Desempeño de técnicos (RF14 + RF15).</summary>
    [HttpGet("tecnicos")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ReporteTecnicoDto>>), 200)]
    public async Task<IActionResult> Tecnicos()
    {
        var result = await _mediator.Send(new ReporteTecnicosQuery());
        return Ok(ApiResponse<IReadOnlyList<ReporteTecnicoDto>>.Ok(result));
    }

    /// <summary>Bitácora de operaciones (RF17).</summary>
    [HttpGet("bitacora")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<BitacoraDto>>), 200)]
    public async Task<IActionResult> Bitacora(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? tabla = null)
    {
        var result = await _mediator.Send(new BitacoraQuery(page, pageSize, tabla));
        return Ok(ApiResponse<IReadOnlyList<BitacoraDto>>.Ok(result));
    }
}
