using FIS.Domain.Services;

namespace FIS.Domain.Tests.Services;

public class CalculadoraMoraServiceTests
{
    private readonly CalculadoraMoraService _sut = new();

    [Fact]
    public void Sin_mora_si_pago_es_antes_o_igual_dia_11()
    {
        var fecha = new DateTime(2026, 1, 11);
        var resultado = _sut.CalcularMora(montoBase: 250m, fecha);
        Assert.Equal(0m, resultado);
    }

    [Fact]
    public void Con_mora_10_porciento_si_pago_es_despues_dia_11()
    {
        var fecha = new DateTime(2026, 1, 12);
        var resultado = _sut.CalcularMora(montoBase: 250m, fecha);
        Assert.Equal(25m, resultado);
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(11, false)]
    [InlineData(12, true)]
    [InlineData(31, true)]
    public void Debe_aplicarse_mora_funciona_para_todos_los_dias(int dia, bool esperado)
    {
        var fecha = new DateTime(2026, 5, dia);
        Assert.Equal(esperado, _sut.DebeAplicarseMora(fecha));
    }
}
