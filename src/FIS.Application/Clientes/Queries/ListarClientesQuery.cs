using FIS.Contracts.Clientes;
using FIS.Contracts.Common;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Clientes.Queries;

public record ListarClientesQuery(string? Filtro, int Page, int PageSize)
    : IRequest<PagedResult<ClienteDto>>;

public class ListarClientesQueryHandler
    : IRequestHandler<ListarClientesQuery, PagedResult<ClienteDto>>
{
    private readonly IClienteRepository _repo;

    public ListarClientesQueryHandler(IClienteRepository repo) => _repo = repo;

    public async Task<PagedResult<ClienteDto>> Handle(
        ListarClientesQuery q, CancellationToken ct)
    {
        var page = q.Page < 1 ? 1 : q.Page;
        var pageSize = q.PageSize is < 1 or > 200 ? 25 : q.PageSize;

        var (items, total) = await _repo.ListarAsync(q.Filtro, page, pageSize, ct);

        return new PagedResult<ClienteDto>
        {
            Page = page,
            PageSize = pageSize,
            Total = total,
            Items = items.Select(c => new ClienteDto
            {
                IdCliente = c.IdCliente,
                TipoCliente = c.TipoCliente,
                CodigoCliente = c.CodigoCliente,
                NombreRazonSocial = c.NombreRazonSocial,
                NitCi = c.NitCi,
                Email = c.Email,
                Telefono = c.Telefono,
                Direccion = c.Direccion,
                Ciudad = c.Ciudad,
                Activo = c.Activo,
                FechaRegistro = c.FechaRegistro
            }).ToList()
        };
    }
}
