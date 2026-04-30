namespace FIS.Domain.Entities;

/// <summary>
/// Mapea dbo.PLAN_SERVICIO. Absorbe TIPO_CONTRATO mediante TipoServicio.
/// </summary>
public class PlanServicio
{
    public int IdPlan { get; private set; }
    public string NombrePlan { get; private set; } = default!;
    public string TipoServicio { get; private set; } = default!;
    public decimal? VelocidadBajadaMbps { get; private set; }
    public decimal? VelocidadSubidaMbps { get; private set; }
    public decimal PrecioMensual { get; private set; }
    public bool Activo { get; private set; } = true;

    public ICollection<Contrato> Contratos { get; private set; } = new List<Contrato>();

    private PlanServicio() { }

    public PlanServicio(
        string nombrePlan, string tipoServicio,
        decimal? velocidadBajada, decimal? velocidadSubida,
        decimal precioMensual)
    {
        NombrePlan = nombrePlan;
        TipoServicio = tipoServicio;
        VelocidadBajadaMbps = velocidadBajada;
        VelocidadSubidaMbps = velocidadSubida;
        PrecioMensual = precioMensual;
        Activo = true;
    }

    public void Actualizar(
        string nombrePlan, string tipoServicio,
        decimal? velocidadBajada, decimal? velocidadSubida,
        decimal precioMensual)
    {
        NombrePlan = nombrePlan;
        TipoServicio = tipoServicio;
        VelocidadBajadaMbps = velocidadBajada;
        VelocidadSubidaMbps = velocidadSubida;
        PrecioMensual = precioMensual;
    }

    public void ActualizarPrecio(decimal nuevoPrecio) => PrecioMensual = nuevoPrecio;
    public void Desactivar() => Activo = false;
}
