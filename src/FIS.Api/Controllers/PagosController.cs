using Asp.Versioning;
using FIS.Application.Pagos.Commands;
using FIS.Application.Pagos.Queries;
using FIS.Contracts.Common;
using FIS.Contracts.Pagos;
using FIS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIS.Api.Controllers;

/// <summary>Registro y anulación de pagos (RF06, RF07, RF08).</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/pagos")]
[Authorize]
public class PagosController : ControllerBase
{
    private readonly IMediator _mediator;
    public PagosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Roles = $"{Roles.Administrador},{Roles.Cajero}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PagoDto>>), 200)]
    public async Task<IActionResult> Listar([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new ListarPagosQuery(page, pageSize));
        return Ok(ApiResponse<PagedResult<PagoDto>>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Administrador},{Roles.Cajero}")]
    [ProducesResponseType(typeof(ApiResponse<PagoDto>), 201)]
    public async Task<IActionResult> Registrar([FromBody] RegistrarPagoRequest request)
    {
        var result = await _mediator.Send(new RegistrarPagoCommand(request));
        return StatusCode(201, ApiResponse<PagoDto>.Ok(result, "Pago registrado."));
    }

    [HttpPost("anular")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Anular([FromBody] AnularPagoRequest request)
    {
        await _mediator.Send(new AnularPagoCommand(request));
        return NoContent();
    }
}
