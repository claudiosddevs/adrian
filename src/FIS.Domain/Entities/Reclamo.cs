using FIS.Domain.Common;
using FIS.Domain.Enums;

namespace FIS.Domain.Entities;

public class Reclamo
{
    private const int MaxReclamosPorTecnico = 5;

    public int IdReclamo { get; private set; }
    public int IdCliente { get; private set; }
    public int? IdContrato { get; private set; }
    public int? IdTecnico { get; private set; }
    public int IdUsuarioRegistro { get; private set; }
    public string NumeroReclamo { get; private set; } = default!;
    public string TipoReclamo { get; private set; } = default!;
    public string Estado { get; private set; } = EstadoReclamo.Recepcionado;
    public string DescripcionProblema { get; private set; } = default!;
    public string? SolucionAplicada { get; private set; }
    public byte Prioridad { get; private set; } = 3;
    public string CanalEntrada { get; private set; } = Enums.CanalEntrada.Telefono;
    public DateTime FechaApertura { get; private set; }
    public DateTime? FechaCierre { get; private set; }
    public byte? Calificacion { get; private set; }
    public int? TiempoRespuestaMin { get; private set; }
    public string? RutaAudio { get; private set; }
    public int? DuracionAudioSeg { get; private set; }
    public string? HashSha256 { get; private set; }

    public Cliente Cliente { get; private set; } = default!;
    public Contrato? Contrato { get; private set; }
    public Usuario? Tecnico { get; private set; }
    public Usuario UsuarioRegistro { get; private set; } = default!;

    private Reclamo() { }

    public Reclamo(
        int idCliente, int? idContrato, int idUsuarioRegistro,
        string numeroReclamo, string tipoReclamo,
        string descripcionProblema, byte prioridad, string canalEntrada)
    {
        if (prioridad < 1 || prioridad > 5)
            throw new BusinessException("La prioridad debe estar entre 1 y 5.");

        IdCliente = idCliente;
        IdContrato = idContrato;
        IdUsuarioRegistro = idUsuarioRegistro;
        NumeroReclamo = numeroReclamo;
        TipoReclamo = tipoReclamo;
        DescripcionProblema = descripcionProblema;
        Prioridad = prioridad;
        CanalEntrada = canalEntrada;
        Estado = EstadoReclamo.Recepcionado;
        FechaApertura = DateTime.UtcNow;
    }

    public void AsignarTecnico(int idTecnico, int reclamosActivosDelTecnico)
    {
        if (reclamosActivosDelTecnico >= MaxReclamosPorTecnico)
            throw new BusinessException(
                $"El técnico ha alcanzado el límite de {MaxReclamosPorTecnico} reclamos activos.");

        IdTecnico = idTecnico;
        if (Estado == EstadoReclamo.Recepcionado)
            Estado = EstadoReclamo.EnProceso;
    }

    public void CambiarEstado(string nuevoEstado)
    {
        Estado = nuevoEstado;
        if (nuevoEstado == EstadoReclamo.Cerrado)
            FechaCierre = DateTime.UtcNow;
    }

    public void Clasificar(string tipo) => TipoReclamo = tipo;

    public void RegistrarSolucion(string solucion)
    {
        SolucionAplicada = solucion;
        Estado = EstadoReclamo.Cerrado;
        FechaCierre = DateTime.UtcNow;
    }

    public void Calificar(byte calificacion, int tiempoRespuestaMin)
    {
        if (calificacion < 1 || calificacion > 5)
            throw new BusinessException("La calificación debe estar entre 1 y 5.");
        Calificacion = calificacion;
        TiempoRespuestaMin = tiempoRespuestaMin;
    }

    public void AdjuntarAudio(string ruta, int duracionSeg, string hashSha256)
    {
        RutaAudio = ruta;
        DuracionAudioSeg = duracionSeg;
        HashSha256 = hashSha256;
    }
}
