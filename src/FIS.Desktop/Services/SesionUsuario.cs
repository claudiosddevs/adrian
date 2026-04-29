using FIS.Contracts.Auth;

namespace FIS.Desktop.Services;

/// <summary>Singleton con los datos del usuario autenticado.</summary>
public class SesionUsuario
{
    public UserInfo? Actual { get; private set; }

    public bool EsAdministrador => Actual?.Rol == "Administrador";
    public bool EsCajero => Actual?.Rol == "Cajero";
    public bool EsTecnico => Actual?.Rol == "Tecnico";

    public void Establecer(UserInfo usuario) => Actual = usuario;
    public void Limpiar() => Actual = null;
}
