using System.Text.Json;

namespace MauiApp1.UnitTests.Models;

/// <summary>
/// Pruebas de HorarioDto.
/// </summary>
public class HorarioTests
{
    [Fact(DisplayName = "HorarioDto: deserializa fila de horario semanal")]
    public void Horario_Deserialize()
    {
        const string json = """
        {
           "horarioID": 1, "grupoID": 5, "materiaID": 10, "docenteID": 7,
           "diaSemana": 1, "diaNombre": "Lunes",
           "horaInicio": "08:00", "horaFin": "10:00",
           "materiaNombre": "Álgebra", "materiaCodigo": "MAT101",
           "grupoCodigo": "GA-1", "docente": "Ing. López", "aula": "A-101"
        }
        """;
        var h = JsonSerializer.Deserialize<HorarioDto>(json)!;
        h.DiaSemana.Should().Be(1);
        h.DiaNombre.Should().Be("Lunes");
        h.HoraInicio.Should().Be("08:00");
        h.HoraFin.Should().Be("10:00");
        h.MateriaNombre.Should().Be("Álgebra");
        h.Aula.Should().Be("A-101");
    }

    [Theory(DisplayName = "HorarioDto: DiaSemana entre 1-7")]
    [InlineData(1)][InlineData(3)][InlineData(7)]
    public void Horario_Valida_DiaSemana(int dia)
    {
        var h = new HorarioDto { DiaSemana = dia };
        h.DiaSemana.Should().BeInRange(1, 7);
    }
}
