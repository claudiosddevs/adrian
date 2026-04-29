namespace FIS.Domain.Services;

/// <summary>
/// Servicio de dominio que centraliza la regla de negocio de mora:
/// si el pago se registra después del día 11, se aplica un 10% de recargo.
/// </summary>
public class CalculadoraMoraService
{
    private const int DiaCorte = 11;
    private const decimal PorcentajeMora = 0.10m;

    public decimal CalcularMora(decimal montoBase, DateTime fechaPago)
    {
        if (fechaPago.Day > DiaCorte)
            return Math.Round(montoBase * PorcentajeMora, 2);
        return 0m;
    }

    public bool DebeAplicarseMora(DateTime fechaPago) => fechaPago.Day > DiaCorte;
}
