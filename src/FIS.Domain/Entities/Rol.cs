namespace FIS.Domain.Entities;

/// <summary>
/// Mapea dbo.ROL.
/// </summary>
public class Rol
{
    public byte IdRol { get; private set; }
    public string NombreRol { get; private set; } = default!;
    public bool Activo { get; private set; } = true;

    public ICollection<Usuario> Usuarios { get; private set; } = new List<Usuario>();

    private Rol() { }

    public Rol(string nombreRol)
    {
        NombreRol = nombreRol ?? throw new ArgumentNullException(nameof(nombreRol));
        Activo = true;
    }

    public void Activar() => Activo = true;
    public void Desactivar() => Activo = false;
    public void Renombrar(string nuevoNombre) => NombreRol = nuevoNombre;
}
