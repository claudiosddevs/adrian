namespace FIS.Domain.Enums;

public static class EstadoReclamo
{
    public const string Recepcionado = "Recepcionado";
    public const string EnProceso = "En Proceso";
    public const string Observado = "Observado";
    public const string Cerrado = "Cerrado";
}

public static class TipoReclamo
{
    public const string Leve = "Leve";
    public const string Medio = "Medio";
    public const string Complejo = "Complejo";
}

public static class CanalEntrada
{
    public const string Telefono = "telefono";
    public const string Web = "web";
    public const string Presencial = "presencial";
    public const string App = "app";
}
