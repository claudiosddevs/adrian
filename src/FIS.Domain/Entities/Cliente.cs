using FIS.Domain.Common;

namespace FIS.Domain.Entities;

/// <summary>
/// Mapea dbo.CLIENTE. tipo_cliente = 'N' (Natural) | 'J' (Jurídico).
/// </summary>
public class Cliente
{
    public int IdCliente { get; private set; }
    public string TipoCliente { get; private set; } = default!;
    public string CodigoCliente { get; private set; } = default!;
    public string NombreRazonSocial { get; private set; } = default!;
    public string NitCi { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Telefono { get; private set; } = default!;
    public string Direccion { get; private set; } = default!;
    public string Ciudad { get; private set; } = default!;
    public bool Activo { get; private set; } = true;
    public DateTime FechaRegistro { get; private set; }

    public ICollection<Contrato> Contratos { get; private set; } = new List<Contrato>();
    public ICollection<Reclamo> Reclamos { get; private set; } = new List<Reclamo>();

    private Cliente() { }

    public Cliente(
        string tipoCliente, string codigoCliente, string nombreRazonSocial,
        string nitCi, string email, string telefono, string direccion, string ciudad)
    {
        TipoCliente = tipoCliente;
        CodigoCliente = codigoCliente;
        NombreRazonSocial = nombreRazonSocial;
        NitCi = nitCi;
        Email = email;
        Telefono = telefono;
        Direccion = direccion;
        Ciudad = ciudad;
        Activo = true;
        FechaRegistro = DateTime.UtcNow.Date;
    }

    public void Actualizar(
        string nombreRazonSocial, string email, string telefono,
        string direccion, string ciudad)
    {
        NombreRazonSocial = nombreRazonSocial;
        Email = email;
        Telefono = telefono;
        Direccion = direccion;
        Ciudad = ciudad;
    }

    public void Desactivar()
    {
        if (Contratos.Any(c => c.Estado == Enums.EstadoContrato.Activo))
            throw new BusinessException(
                "No se puede desactivar el cliente. Tiene contratos activos.");
        Activo = false;
    }
}
