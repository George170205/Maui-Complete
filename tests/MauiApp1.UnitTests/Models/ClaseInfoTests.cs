namespace MauiApp1.UnitTests.Models;

/// <summary>
/// Pruebas de la lógica derivada de <c>ClaseInfo.Iniciales</c>.
/// Misma regla que <c>Materia.Iniciales</c> pero sobre otra entidad.
/// </summary>
public class ClaseInfoTests
{
    [Fact(DisplayName = "ClaseInfo.Iniciales: vacío si nombre es nulo/blanco")]
    public void Iniciales_EmptyWhenBlank()
    {
        new ClaseInfo { Nombre = "" }.Iniciales.Should().BeEmpty();
        new ClaseInfo { Nombre = "    " }.Iniciales.Should().BeEmpty();
    }

    [Theory(DisplayName = "ClaseInfo.Iniciales: un nombre => 1 letra; varias => 2 letras")]
    [InlineData("Algebra", "A")]
    [InlineData("algoritmos estructuras datos", "AE")]
    [InlineData("sistemas operativos", "SO")]
    public void Iniciales_Cases(string nombre, string expected)
    {
        new ClaseInfo { Nombre = nombre }.Iniciales.Should().Be(expected);
    }

    [Fact(DisplayName = "ClaseInfo: acepta porcentaje en rango 0-100")]
    public void ClaseInfo_Porcentaje_InRange()
    {
        var c = new ClaseInfo { Nombre = "X", Porcentaje = 85 };
        c.Porcentaje.Should().BeInRange(0, 100);
    }
}
