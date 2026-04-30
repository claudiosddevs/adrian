namespace FIS.Contracts.Reportes;

public class ReporteMoraDto
{
    public int IdCliente { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public int IdContrato { get; set; }
    public string NombrePlan { get; set; } = string.Empty;
    public decimal MontoMora { get; set; }
    public int DiasAtraso { get; set; }
}

public class ReporteVentasDto
{
    public int Mes { get; set; }
    public int Anio { get; set; }
    public string NombreMes { get; set; } = string.Empty;
    public int TotalContratos { get; set; }
    public decimal TotalIngresos { get; set; }
    public int ContratosActivos { get; set; }
}

public class ReporteTecnicoDto
{
    public int IdUsuario { get; set; }
    public string NombreTecnico { get; set; } = string.Empty;
    public int TotalReclamos { get; set; }
    public int ReclamosResueltos { get; set; }
    public int ReclamosPendientes { get; set; }
    public double PorcentajeResolucion { get; set; }
    public double CalificacionPromedio { get; set; }
}

public class BitacoraDto
{
    public int IdBitacora { get; set; }
    public string Tabla { get; set; } = string.Empty;
    public string Operacion { get; set; } = string.Empty;
    public string? ValoresAnteriores { get; set; }
    public string? ValoresNuevos { get; set; }
    public string? UsuarioAccion { get; set; }
    public DateTime FechaHora { get; set; }
}
