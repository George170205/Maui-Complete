using System.Text.Json;

namespace MauiApp1.UnitTests.Models;

/// <summary>
/// Pruebas de DocenteDto y ProximaAsistenciaDto.
/// </summary>
public class DocenteAndProximaTests
{
    [Fact(DisplayName = "DocenteDto: deserializa lista de docentes")]
    public void Docente_Deserialize()
    {
        const string json = """
        {"docenteID":1, "usuarioID":10, "nombre":"Ana", "apellido":"Ruiz",
         "email":"a.ruiz@uni.mx", "numeroEmpleado":"E-001", "departamento":"ISC",
         "tituloAcademico":"Dra.", "especialidad":"IA", "gruposAsignados":3}
        """;

        var d = JsonSerializer.Deserialize<DocenteDto>(json)!;
        d.DocenteID.Should().Be(1);
        d.Nombre.Should().Be("Ana");
        d.TituloAcademico.Should().Be("Dra.");
        d.GruposAsignados.Should().Be(3);
    }

    [Fact(DisplayName = "ProximaAsistenciaDto: deserializa sesión con flag YaRegistrada")]
    public void ProximaAsistencia_Deserialize()
    {
        const string json = """
        {"sesionClaseID":50, "grupoID":5, "materiaID":10, "materiaNombre":"BD",
         "profesor":"Ing. Gómez", "fecha":"2026-04-22T09:00:00Z",
         "horaInicio":"09:00", "horaFin":"10:30", "aula":"A-202",
         "tema":"Joins", "yaRegistrada":true, "estadoAsistencia":"Presente"}
        """;

        var p = JsonSerializer.Deserialize<ProximaAsistenciaDto>(json)!;
        p.MateriaNombre.Should().Be("BD");
        p.YaRegistrada.Should().BeTrue();
        p.EstadoAsistencia.Should().Be("Presente");
    }

    [Fact(DisplayName = "ProximaAsistenciaDto: default YaRegistrada = false")]
    public void Proxima_Default_NotRegistered()
    {
        new ProximaAsistenciaDto().YaRegistrada.Should().BeFalse();
    }
}
