using FIS.Domain.Common;

namespace FIS.Domain.Entities;

/// <summary>
/// Mapea dbo.USUARIO. Absorbe TÉCNICO mediante el campo Especialidad.
/// </summary>
public class Usuario
{
    private const int MaxIntentosFallidos = 5;
    private const int MinutosBloqueo = 30;

    public int IdUsuario { get; private set; }
    public byte IdRol { get; private set; }
    public string Username { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Nombres { get; private set; } = default!;
    public string Apellidos { get; private set; } = default!;
    public string? Especialidad { get; private set; }
    public bool Activo { get; private set; } = true;
    public byte IntentosFallidos { get; private set; }
    public DateTime? BloqueadoHasta { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Rol Rol { get; private set; } = default!;

    private Usuario() { }

    public Usuario(
        byte idRol, string username, string passwordHash, string email,
        string nombres, string apellidos, string? especialidad = null)
    {
        IdRol = idRol;
        Username = username;
        PasswordHash = passwordHash;
        Email = email;
        Nombres = nombres;
        Apellidos = apellidos;
        Especialidad = especialidad;
        Activo = true;
        CreatedAt = DateTime.UtcNow;
    }

    public bool EstaBloqueado() =>
        BloqueadoHasta.HasValue && BloqueadoHasta.Value > DateTime.UtcNow;

    public void RegistrarIntentoFallido()
    {
        IntentosFallidos++;
        if (IntentosFallidos >= MaxIntentosFallidos)
            BloqueadoHasta = DateTime.UtcNow.AddMinutes(MinutosBloqueo);
    }

    public void RegistrarLoginExitoso()
    {
        IntentosFallidos = 0;
        BloqueadoHasta = null;
    }

    public void CambiarRol(byte idRol) => IdRol = idRol;
    public void CambiarPasswordHash(string nuevoHash) => PasswordHash = nuevoHash;
    public void Activar() => Activo = true;

    public void Desactivar()
    {
        if (!Activo)
            throw new BusinessException("El usuario ya está desactivado.");
        Activo = false;
    }
}
