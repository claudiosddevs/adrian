namespace FIS.Contracts.Clientes;

public class ClienteDto
{
    public int IdCliente { get; set; }
    public string TipoCliente { get; set; } = string.Empty;
    public string CodigoCliente { get; set; } = string.Empty;
    public string NombreRazonSocial { get; set; } = string.Empty;
    public string NitCi { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
}

public class CrearClienteRequest
{
    public string TipoCliente { get; set; } = "N";
    public string NombreRazonSocial { get; set; } = string.Empty;
    public string NitCi { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
}
