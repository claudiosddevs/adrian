using FIS.Application.Common.Interfaces;
using FIS.Contracts.Reportes;
using FIS.Domain.Enums;
using FIS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FIS.Infrastructure.Services;

public class ReporteService : IReporteService
{
    private readonly FisDbContext _ctx;
    public ReporteService(FisDbContext ctx) => _ctx = ctx;

    public async Task<IReadOnlyList<ReporteMoraDto>> GetReporteMoraAsync(CancellationToken ct = default)
    {
        var hoy = DateTime.UtcNow;
        if (hoy.Day <= 12) return new List<ReporteMoraDto>();

        return await _ctx.Contratos
            .Include(c => c.Cliente)
            .Include(c => c.Plan)
            .Where(c => c.Estado == EstadoContrato.Activo)
            .Select(c => new ReporteMoraDto
            {
                IdCliente = c.IdCliente,
                NombreCliente = c.Cliente.NombreRazonSocial,
                Telefono = c.Cliente.Telefono,
                IdContrato = c.IdContrato,
                NombrePlan = c.Plan.NombrePlan,
                MontoMora = c.Plan.PrecioMensual * 0.10m,
                DiasAtraso = hoy.Day - 12
            })
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ReporteVentasDto>> GetReporteVentasAsync(int anio, CancellationToken ct = default)
    {
        var datos = await _ctx.Contratos
            .Include(c => c.Plan)
            .Where(c => c.FechaInicio.Year == anio)
            .GroupBy(c => c.FechaInicio.Month)
            .Select(g => new
            {
                Mes = g.Key,
                Total = g.Count(),
                Activos = g.Count(c => c.Estado == EstadoContrato.Activo),
                Ingresos = g.Sum(c => c.Plan.PrecioMensual)
            })
            .OrderBy(x => x.Mes)
            .ToListAsync(ct);

        var nombresMes = new[]
        {
            "", "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
            "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
        };

        return datos.Select(d => new ReporteVentasDto
        {
            Mes = d.Mes,
            Anio = anio,
            NombreMes = nombresMes[d.Mes],
            TotalContratos = d.Total,
            ContratosActivos = d.Activos,
            TotalIngresos = d.Ingresos
        }).ToList();
    }

    public async Task<IReadOnlyList<ReporteTecnicoDto>> GetReporteTecnicosAsync(CancellationToken ct = default)
    {
        return await _ctx.Reclamos
            .Where(r => r.IdTecnico != null)
            .GroupBy(r => new { r.IdTecnico, r.Tecnico!.Nombres, r.Tecnico.Apellidos })
            .Select(g => new ReporteTecnicoDto
            {
                IdUsuario = g.Key.IdTecnico!.Value,
                NombreTecnico = $"{g.Key.Nombres} {g.Key.Apellidos}",
                TotalReclamos = g.Count(),
                ReclamosResueltos = g.Count(r => r.Estado == EstadoReclamo.Cerrado),
                ReclamosPendientes = g.Count(r => r.Estado != EstadoReclamo.Cerrado),
                PorcentajeResolucion = g.Count() == 0 ? 0 :
                    (double)g.Count(r => r.Estado == EstadoReclamo.Cerrado) / g.Count() * 100,
                CalificacionPromedio = (double?)g.Average(r => (double?)r.Calificacion) ?? 0
            })
            .OrderByDescending(x => x.PorcentajeResolucion)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<BitacoraDto>> GetBitacoraAsync(
        int page, int pageSize, string? tabla, CancellationToken ct = default)
    {
        var query = _ctx.Bitacora.AsQueryable();
        if (!string.IsNullOrWhiteSpace(tabla))
            query = query.Where(b => b.Tabla == tabla);

        return await query
            .OrderByDescending(b => b.FechaHora)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BitacoraDto
            {
                IdBitacora = b.IdBitacora,
                Tabla = b.Tabla,
                Operacion = b.Operacion,
                ValoresAnteriores = b.ValoresAnteriores,
                ValoresNuevos = b.ValoresNuevos,
                UsuarioAccion = b.UsuarioAccion,
                FechaHora = b.FechaHora
            })
            .ToListAsync(ct);
    }
}
