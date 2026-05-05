using System.Text.Json;

namespace MauiApp1.UnitTests.Models;

/// <summary>
/// Pruebas del DTO <c>KardexItemDto</c> (historial académico).
/// </summary>
public class KardexTests
{
    [Fact(DisplayName = "KardexItemDto: deserializa cálculo de % de asistencia")]
    public void KardexItem_Deserialize_ComputesAsistencia()
    {
        const string json = """
        {
            "inscripcionID": 1,
            "materiaID": 10,
            "codigo": "MAT101",
            "nombre": "Matemáticas",
            "ciclo": "2026-1",
            "calificacionFinal": 9.5,
            "sesionesTotales": 40,
            "sesionesAsistidas": 36,
            "porcentajeAsistencia": 90.0,
            "estado": "Aprobado"
        }
        """;

        var k = JsonSerializer.Deserialize<KardexItemDto>(json)!;
        k.CalificacionFinal.Should().Be(9.5m);
        k.SesionesTotales.Should().Be(40);
        k.SesionesAsistidas.Should().Be(36);
        k.PorcentajeAsistencia.Should().Be(90.0m);
        k.Estado.Should().Be("Aprobado");
    }

    [Fact(DisplayName = "KardexItemDto: admite calificación nula (materia en curso)")]
    public void KardexItem_AllowsNullCalificacion()
    {
        var k = new KardexItemDto { Nombre = "En curso", CalificacionFinal = null };
        k.CalificacionFinal.Should().BeNull();
    }
}
