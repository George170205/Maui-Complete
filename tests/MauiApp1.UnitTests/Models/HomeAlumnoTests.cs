using System.Text.Json;

namespace MauiApp1.UnitTests.Models;

/// <summary>
/// Pruebas de HomeAlumnoDto y sus anidados (ProximaClaseDto, MateriaActivaDto).
/// </summary>
public class HomeAlumnoTests
{
    [Fact(DisplayName = "HomeAlumnoDto: defaults de colecciones no son null")]
    public void HomeAlumno_Defaults()
    {
        var h = new HomeAlumnoDto();
        h.NombreCompleto.Should().BeEmpty();
        h.Iniciales.Should().BeEmpty();
        h.Matricula.Should().BeEmpty();
        h.MateriasList.Should().NotBeNull().And.BeEmpty();
        h.ProximaClase.Should().BeNull();
        h.Promedio.Should().Be(0m);
        h.PorcentajeAsistencia.Should().Be(0);
        h.FaltasTotales.Should().Be(0);
    }

    [Fact(DisplayName = "HomeAlumnoDto: deserializa payload completo")]
    public void HomeAlumno_Deserialize_Full()
    {
        const string json = """
        {
          "estudianteID": 33,
          "nombreCompleto": "Juan Pérez",
          "iniciales": "JP",
          "matricula": "A1234",
          "carrera": "ISC",
          "semestre": 5,
          "email": "juan@test.com",
          "materiasActivas": 6,
          "promedio": 8.75,
          "porcentajeAsistencia": 92,
          "faltasTotales": 3,
          "proximaClase": {
            "sesionClaseID": 100,
            "grupoID": 12,
            "nombre": "Base de Datos",
            "salon": "A-101",
            "fecha": "2026-04-21T09:00:00Z",
            "horaInicio": "09:00",
            "horaFin": "10:30",
            "profesor": "Ing. Gómez"
          },
          "materiasList": [
             { "materiaID":1, "grupoID":1, "nombre":"BD", "codigo":"BD1", "porcentaje":90 },
             { "materiaID":2, "grupoID":2, "nombre":"SO", "codigo":"SO1", "porcentaje":85 }
          ]
        }
        """;

        var h = JsonSerializer.Deserialize<HomeAlumnoDto>(json)!;
        h.NombreCompleto.Should().Be("Juan Pérez");
        h.Iniciales.Should().Be("JP");
        h.MateriasActivas.Should().Be(6);
        h.Promedio.Should().Be(8.75m);
        h.PorcentajeAsistencia.Should().Be(92);
        h.ProximaClase.Should().NotBeNull();
        h.ProximaClase!.Nombre.Should().Be("Base de Datos");
        h.MateriasList.Should().HaveCount(2);
        h.MateriasList[0].Nombre.Should().Be("BD");
    }
}
