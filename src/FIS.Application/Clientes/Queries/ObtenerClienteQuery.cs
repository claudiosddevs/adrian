using FIS.Application.Clientes.Commands;
using FIS.Contracts.Clientes;
using FIS.Domain.Common;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Clientes.Queries;

public record ObtenerClienteQuery(int Id) : IRequest<ClienteDto>;

public class ObtenerClienteQueryHandler : IRequestHandler<ObtenerClienteQuery, ClienteDto>
{
    private readonly IClienteRepository _repo;
    public ObtenerClienteQueryHandler(IClienteRepository repo) => _repo = repo;

    public async Task<ClienteDto> Handle(ObtenerClienteQuery query, CancellationToken ct)
    {
        var cliente = await _repo.GetByIdAsync(query.Id, ct)
            ?? throw new BusinessException($"Cliente con ID {query.Id} no encontrado.");

        return CrearClienteCommandHandler.MapToDto(cliente);
    }
}
