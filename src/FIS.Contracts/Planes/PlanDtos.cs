namespace FIS.Contracts.Planes;

public class PlanDto
{
    public int IdPlan { get; set; }
    public string NombrePlan { get; set; } = string.Empty;
    public string TipoServicio { get; set; } = string.Empty;
    public decimal? VelocidadBajadaMbps { get; set; }
    public decimal? VelocidadSubidaMbps { get; set; }
    public decimal PrecioMensual { get; set; }
    public bool Activo { get; set; }
}

public class CrearPlanRequest
{
    public string NombrePlan { get; set; } = string.Empty;
    public string TipoServicio { get; set; } = string.Empty;
    public decimal? VelocidadBajadaMbps { get; set; }
    public decimal? VelocidadSubidaMbps { get; set; }
    public decimal PrecioMensual { get; set; }
}
