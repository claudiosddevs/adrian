namespace FIS.Contracts.Contratos;

public class ContratoDto
{
    public int IdContrato { get; set; }
    public int ClienteId { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public int PlanId { get; set; }
    public string NombrePlan { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string MetodoPago { get; set; } = string.Empty;
    public decimal MontoTotal { get; set; }
}

public class RegistrarContratoRequest
{
    public int ClienteId { get; set; }
    public int PlanId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string MetodoPago { get; set; } = "Mensual";
}
