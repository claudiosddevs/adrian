using FIS.Domain.Common;

namespace FIS.Domain.Entities;

public class Pago
{
    public int IdPago { get; private set; }
    public int IdContrato { get; private set; }
    public int IdCajero { get; private set; }
    public string MetodoPago { get; private set; } = default!;
    public string NumeroRecibo { get; private set; } = default!;
    public byte PeriodoMes { get; private set; }
    public short PeriodoAnio { get; private set; }
    public DateTime FechaPago { get; private set; }
    public decimal MontoTotal { get; private set; }
    public decimal MontoMora { get; private set; }
    public string? Conceptos { get; private set; }
    public bool Anulado { get; private set; }
    public string? MotivoAnulacion { get; private set; }
    public DateTime? FechaAnulacion { get; private set; }

    public Contrato Contrato { get; private set; } = default!;
    public Usuario Cajero { get; private set; } = default!;

    private Pago() { }

    public Pago(
        int idContrato, int idCajero, string metodoPago, string numeroRecibo,
        byte periodoMes, short periodoAnio,
        decimal montoTotal, decimal montoMora, string? conceptosJson)
    {
        if (montoTotal <= 0)
            throw new BusinessException("El monto total debe ser mayor a cero.");

        IdContrato = idContrato;
        IdCajero = idCajero;
        MetodoPago = metodoPago;
        NumeroRecibo = numeroRecibo;
        PeriodoMes = periodoMes;
        PeriodoAnio = periodoAnio;
        MontoTotal = montoTotal;
        MontoMora = montoMora;
        Conceptos = conceptosJson;
        FechaPago = DateTime.UtcNow;
        Anulado = false;
    }

    public void Anular(string motivo)
    {
        if (Anulado)
            throw new BusinessException("El pago ya fue anulado.");
        if (string.IsNullOrWhiteSpace(motivo))
            throw new BusinessException("Debe indicar el motivo de la anulación.");

        Anulado = true;
        MotivoAnulacion = motivo;
        FechaAnulacion = DateTime.UtcNow;
    }
}
