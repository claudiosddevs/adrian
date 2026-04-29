using FIS.Domain.Common;
using FIS.Domain.Entities;
using FIS.Domain.Enums;

namespace FIS.Domain.Tests.Entities;

public class ContratoTests
{
    [Fact]
    public void Contrato_debe_durar_minimo_1_anio()
    {
        var inicio = new DateTime(2026, 1, 1);
        var fin = inicio.AddMonths(11);

        var ex = Assert.Throws<BusinessException>(() =>
            new Contrato(
                idCliente: 1, idPlan: 1, idUsuarioRegistro: 1,
                numeroContrato: "CT-001",
                fechaInicio: inicio, fechaFin: fin));

        Assert.Contains("1 año", ex.Message);
    }

    [Fact]
    public void Contrato_se_crea_con_estado_activo()
    {
        var contrato = new Contrato(
            idCliente: 1, idPlan: 1, idUsuarioRegistro: 1,
            numeroContrato: "CT-001",
            fechaInicio: DateTime.Today,
            fechaFin: DateTime.Today.AddYears(1).AddDays(1));

        Assert.Equal(EstadoContrato.Activo, contrato.Estado);
    }

    [Fact]
    public void Suspender_cambia_estado_a_suspendido()
    {
        var c = new Contrato(1, 1, 1, "CT-001",
            DateTime.Today, DateTime.Today.AddYears(1).AddDays(1));
        c.Suspender();
        Assert.Equal(EstadoContrato.Suspendido, c.Estado);
    }
}
