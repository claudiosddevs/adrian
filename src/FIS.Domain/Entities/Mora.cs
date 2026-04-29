namespace FIS.Domain.Entities;

/// <summary>
/// Mapea dbo.MORA. Relación 1:1 con CONTRATO.
/// </summary>
public class Mora
{
    public int IdMora { get; private set; }
    public int IdContrato { get; private set; }
    public bool EnMora { get; private set; }
    public DateTime? FechaInicioMora { get; private set; }
    public decimal MontoAdeudado { get; private set; }
    public bool ServicioCortado { get; private set; }
    public DateTime? FechaRegularizacion { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Contrato Contrato { get; private set; } = default!;

    private Mora() { }

    public Mora(int idContrato)
    {
        IdContrato = idContrato;
        EnMora = false;
        MontoAdeudado = 0;
        ServicioCortado = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ActivarMora(decimal monto)
    {
        EnMora = true;
        MontoAdeudado = monto;
        FechaInicioMora = DateTime.UtcNow.Date;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CortarServicio()
    {
        ServicioCortado = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Regularizar()
    {
        EnMora = false;
        MontoAdeudado = 0;
        ServicioCortado = false;
        FechaRegularizacion = DateTime.UtcNow.Date;
        UpdatedAt = DateTime.UtcNow;
    }
}
