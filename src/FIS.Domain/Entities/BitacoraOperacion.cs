namespace FIS.Domain.Entities;

/// <summary>
/// Mapea dbo.BITACORA. Registra todas las operaciones de la BD (RF17 / trigger).
/// </summary>
public class BitacoraOperacion
{
    public int IdBitacora { get; private set; }
    public string Tabla { get; private set; } = default!;
    public string Operacion { get; private set; } = default!;
    public string? ValoresAnteriores { get; private set; }
    public string? ValoresNuevos { get; private set; }
    public string? UsuarioAccion { get; private set; }
    public DateTime FechaHora { get; private set; }

    private BitacoraOperacion() { }
}
