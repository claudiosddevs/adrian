using Asp.Versioning;
using FIS.Application.Usuarios.Commands;
using FIS.Application.Usuarios.Queries;
using FIS.Contracts.Common;
using FIS.Contracts.Usuarios;
using FIS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIS.Api.Controllers;

/// <summary>Gestión de Usuarios y Roles (RF16).</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/usuarios")]
[Authorize(Roles = Roles.Administrador)]
public class UsuariosController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsuariosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<UsuarioDto>>), 200)]
    public async Task<IActionResult> Listar()
    {
        var result = await _mediator.Send(new ListarUsuariosQuery());
        return Ok(ApiResponse<IReadOnlyList<UsuarioDto>>.Ok(result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UsuarioDto>), 201)]
    public async Task<IActionResult> Crear([FromBody] CrearUsuarioRequest request)
    {
        var result = await _mediator.Send(new CrearUsuarioCommand(request));
        return StatusCode(201, ApiResponse<UsuarioDto>.Ok(result, "Usuario creado."));
    }
}
