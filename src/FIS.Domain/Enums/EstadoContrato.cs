namespace FIS.Domain.Enums;

public static class EstadoContrato
{
    public const string Activo = "activo";
    public const string Suspendido = "suspendido";
    public const string Finalizado = "finalizado";
    public const string Cancelado = "cancelado";

    public static readonly string[] Todos =
        { Activo, Suspendido, Finalizado, Cancelado };
}
