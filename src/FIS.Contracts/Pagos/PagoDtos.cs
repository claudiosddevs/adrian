namespace FIS.Contracts.Pagos;

public class PagoDto
{
    public int IdPago { get; set; }
    public int ContratoId { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public DateTime FechaPago { get; set; }
    public decimal Monto { get; set; }
    public string MetodoPago { get; set; } = string.Empty;
    public bool Anulado { get; set; }
    public string? MotivoAnulacion { get; set; }
    public decimal? Mora { get; set; }
}

public class RegistrarPagoRequest
{
    public int ContratoId { get; set; }
    public decimal Monto { get; set; }
    public string MetodoPago { get; set; } = "Efectivo";
}

public class AnularPagoRequest
{
    public int IdPago { get; set; }
    public string Motivo { get; set; } = string.Empty;
}
