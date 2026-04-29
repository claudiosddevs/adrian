using FIS.Contracts.Auth;
using MediatR;

namespace FIS.Application.Auth.Login;

public record LoginCommand(string Username, string Password) : IRequest<TokenResponse>;
