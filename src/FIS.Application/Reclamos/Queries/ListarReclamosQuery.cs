using FIS.Application.Reclamos.Commands;
using FIS.Contracts.Common;
using FIS.Contracts.Reclamos;
using FIS.Domain.Interfaces;
using MediatR;

namespace FIS.Application.Reclamos.Queries;

public record ListarReclamosQuery(string? Estado = null, int? TecnicoId = null, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<ReclamoDto>>;

public class ListarReclamosQueryHandler : IRequestHandler<ListarReclamosQuery, PagedResult<ReclamoDto>>
{
    private readonly IReclamoRepository _repo;
    public ListarReclamosQueryHandler(IReclamoRepository repo) => _repo = repo;

    public async Task<PagedResult<ReclamoDto>> Handle(ListarReclamosQuery q, CancellationToken ct)
    {
        var items = await _repo.ListarAsync(q.Estado, q.TecnicoId, q.Page, q.PageSize, ct);
        var total = await _repo.ContarTotalAsync(q.Estado, ct);

        return new PagedResult<ReclamoDto>
        {
            Items = items.Select(r => RegistrarReclamoCommandHandler.MapToDto(r)).ToList(),
            Total = total,
            Page = q.Page,
            PageSize = q.PageSize
        };
    }
}
