using System.Text.Json;

namespace MauiApp1.UnitTests.Models;

/// <summary>
/// Pruebas de <c>Grupo</c> e <c>InscripcionDto</c>.
/// </summary>
public class GrupoTests
{
    [Fact(DisplayName = "Grupo: defaults conformes al contrato del backend")]
    public void Grupo_Defaults()
    {
        var g = new Grupo();
        g.GrupoID.Should().Be(0);
        g.DocenteID.Should().BeNull();
        g.Nombre.Should().BeEmpty();
        g.Ciclo.Should().BeNull();
        g.CupoMaximo.Should().BeNull();
        g.Inscritos.Should().Be(0); // derivado local
    }

    [Fact(DisplayName = "Grupo: deserializa JSON con Materia anidada")]
    public void Grupo_Deserializes_WithNestedMateria()
    {
        const string json = """
        {
          "grupoID": 12,
          "materiaID": 5,
          "docenteID": 7,
          "nombre": "Grupo A",
          "ciclo": "2026-1",
          "cupoMaximo": 30,
          "horario": "L-M 8-10",
          "salon": "B-101",
          "materia": { "MateriaID": 5, "nombre": "Álgebra", "codigo": "MAT" }
        }
        """;

        var g = JsonSerializer.Deserialize<Grupo>(json);
        g.Should().NotBeNull();
        g!.GrupoID.Should().Be(12);
        g.DocenteID.Should().Be(7);
        g.Nombre.Should().Be("Grupo A");
        g.Ciclo.Should().Be("2026-1");
        g.CupoMaximo.Should().Be(30);
        g.Materia.Should().NotBeNull();
        g.Materia!.Nombre.Should().Be("Álgebra");
    }

    [Fact(DisplayName = "InscripcionDto: defaults")]
    public void Inscripcion_Defaults()
    {
        var i = new InscripcionDto();
        i.InscripcionID.Should().Be(0);
        i.Nombre.Should().BeEmpty();
        i.CalificacionFinal.Should().BeNull();
        i.Estado.Should().BeNull();
    }
}
