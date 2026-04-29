using Asp.Versioning;
using FIS.Application.Auth.Login;
using FIS.Contracts.Auth;
using FIS.Contracts.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIS.Api.Controllers;

/// <summary>
/// Endpoints de autenticación (HU01). Emite JWT a usuarios válidos.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>Login con username y password.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<TokenResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var token = await _mediator.Send(new LoginCommand(req.Username, req.Password));
        return Ok(ApiResponse<TokenResponse>.Ok(token, "Inicio de sesión exitoso."));
    }
}
