using System.Text.Json;

namespace MauiApp1.UnitTests.Models;

/// <summary>
/// Pruebas del modelo <c>Materia</c>.
/// Incluye la lógica de la propiedad derivada <c>Iniciales</c>, defaults de
/// propiedades opcionales y serialización JSON (contrato con el backend).
/// </summary>
public class MateriaTests
{
    [Theory(DisplayName = "Iniciales: retorna cadena vacía cuando Nombre es nulo o blanco")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Iniciales_ReturnsEmpty_WhenNombreIsNullOrBlank(string? nombre)
    {
        var m = new Materia { Nombre = nombre ?? string.Empty };
        m.Iniciales.Should().BeEmpty();
    }

    [Theory(DisplayName = "Iniciales: devuelve primera letra mayúscula si hay una palabra")]
    [InlineData("matematicas", "M")]
    [InlineData("fisica", "F")]
    [InlineData("QUIMICA", "Q")]
    public void Iniciales_SingleWord_ReturnsFirstLetterUpper(string nombre, string expected)
    {
        new Materia { Nombre = nombre }.Iniciales.Should().Be(expected);
    }

    [Theory(DisplayName = "Iniciales: devuelve dos iniciales mayúsculas para dos palabras o más")]
    [InlineData("Base de Datos", "BD")]
    [InlineData("programacion movil", "PM")]
    [InlineData("ingenieria de software", "ID")]
    [InlineData("sistemas  operativos", "SO")]
    public void Iniciales_MultiWord_ReturnsTwoUpperInitials(string nombre, string expected)
    {
        new Materia { Nombre = nombre }.Iniciales.Should().Be(expected);
    }

    [Fact(DisplayName = "Materia: valores por defecto correctos")]
    public void Materia_Defaults_AreCorrect()
    {
        var m = new Materia();
        m.MateriaID.Should().Be(0);
        m.Codigo.Should().BeEmpty();
        m.Nombre.Should().BeEmpty();
        m.Descripcion.Should().BeNull();
        m.Creditos.Should().BeNull();
        m.HorasSemana.Should().BeNull();
        m.Activo.Should().BeNull();
        m.Porcentaje.Should().Be(0);
    }

    [Fact(DisplayName = "Materia: deserializa correctamente JSON del backend")]
    public void Materia_DeserializeJson_MapsCorrectly()
    {
        const string json = """
        {
            "MateriaID": 5,
            "codigo": "MAT101",
            "nombre": "Matemáticas I",
            "descripcion": "Álgebra y cálculo",
            "creditos": 6,
            "horasSemana": 4,
            "activo": true,
            "profesor": "Ing. López",
            "horario": "L-M-V 10:00",
            "salon": "A-201"
        }
        """;

        var m = JsonSerializer.Deserialize<Materia>(json);
        m.Should().NotBeNull();
        m!.MateriaID.Should().Be(5);
        m.Codigo.Should().Be("MAT101");
        m.Nombre.Should().Be("Matemáticas I");
        m.Descripcion.Should().Be("Álgebra y cálculo");
        m.Creditos.Should().Be(6);
        m.HorasSemana.Should().Be(4);
        m.Activo.Should().Be(true);
        m.Profesor.Should().Be("Ing. López");
        m.Horario.Should().Be("L-M-V 10:00");
        m.Salon.Should().Be("A-201");
    }
}
