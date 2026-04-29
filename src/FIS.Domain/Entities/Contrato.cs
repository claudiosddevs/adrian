using FIS.Domain.Common;
using FIS.Domain.Enums;

namespace FIS.Domain.Entities;

public class Contrato
{
    public int IdContrato { get; private set; }
    public int IdCliente { get; private set; }
    public int IdPlan { get; private set; }
    public int IdUsuarioRegistro { get; private set; }
    public string NumeroContrato { get; private set; } = default!;
    public DateTime FechaInicio { get; private set; }
    public DateTime FechaFin { get; private set; }
    public string Estado { get; private set; } = EstadoContrato.Activo;
    public DateTime CreatedAt { get; private set; }

    public Cliente Cliente { get; private set; } = default!;
    public PlanServicio Plan { get; private set; } = default!;
    public Usuario UsuarioRegistro { get; private set; } = default!;
    public Mora? Mora { get; private set; }
    public ICollection<Pago> Pagos { get; private set; } = new List<Pago>();

    private Contrato() { }

    public Contrato(
        int idCliente, int idPlan, int idUsuarioRegistro,
        string numeroContrato, DateTime fechaInicio, DateTime fechaFin)
    {
        if (fechaFin <= fechaInicio)
            throw new BusinessException("La fecha fin debe ser mayor a la fecha inicio.");
        if ((fechaFin - fechaInicio).TotalDays < 365)
            throw new BusinessException("La duración mínima del contrato es de 1 año.");

        IdCliente = idCliente;
        IdPlan = idPlan;
        IdUsuarioRegistro = idUsuarioRegistro;
        NumeroContrato = numeroContrato;
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
        Estado = EstadoContrato.Activo;
        CreatedAt = DateTime.UtcNow;
    }

    public void Suspender() => Estado = EstadoContrato.Suspendido;
    public void Reactivar() => Estado = EstadoContrato.Activo;
    public void Finalizar() => Estado = EstadoContrato.Finalizado;
    public void Cancelar() => Estado = EstadoContrato.Cancelado;
}
