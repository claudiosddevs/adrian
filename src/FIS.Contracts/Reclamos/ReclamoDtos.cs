namespace FIS.Contracts.Reclamos;

public class ReclamoDto
{
    public int IdReclamo { get; set; }
    public int ClienteId { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Clasificacion { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public int? TecnicoAsignadoId { get; set; }
    public string? NombreTecnico { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime? FechaCierre { get; set; }
    public string? ObservacionCierre { get; set; }
    public int? Calificacion { get; set; }
}

public class RegistrarReclamoRequest
{
    public int ClienteId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Clasificacion { get; set; } = "Leve";
}

public class AsignarTecnicoRequest
{
    public int TecnicoId { get; set; }
}

public class CambiarEstadoReclamoRequest
{
    public string NuevoEstado { get; set; } = string.Empty;
    public string? Observacion { get; set; }
}
