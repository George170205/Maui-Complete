using System.Text.Json;

namespace MauiApp1.UnitTests.Models;

/// <summary>
/// Pruebas sobre CalificacionDto / Bulk / Upsert.
/// Valida contratos con POST /api/calificaciones/bulk.
/// </summary>
public class CalificacionTests
{
    [Fact(DisplayName = "CalificacionBulkItemDto: PuntosMaximos default 10 y TipoEvaluacion 'Final'")]
    public void BulkItem_Defaults()
    {
        var item = new CalificacionBulkItemDto { InscripcionID = 1, Puntos = 9 };
        item.PuntosMaximos.Should().Be(10);
        item.TipoEvaluacion.Should().Be("Final");
    }

    [Fact(DisplayName = "CalificacionUpsertDto: record mantiene los valores")]
    public void Upsert_RecordValues()
    {
        var u = new CalificacionUpsertDto(1, "Parcial", "P1", 7.5m, 10m);
        u.Puntos.Should().Be(7.5m);
        u.PuntosMaximos.Should().Be(10m);
        u.TipoEvaluacion.Should().Be("Parcial");
    }

    [Fact(DisplayName = "CalificacionBulkDto: serializa correctamente la lista")]
    public void Bulk_SerializesCorrectly()
    {
        var bulk = new CalificacionBulkDto
        {
            Items = new()
            {
                new CalificacionBulkItemDto { InscripcionID = 1, Puntos = 8, PuntosMaximos = 10 },
                new CalificacionBulkItemDto { InscripcionID = 2, Puntos = 9.5m, PuntosMaximos = 10 }
            }
        };
        var json = JsonSerializer.Serialize(bulk);
        json.Should().Contain("\"items\"");
        json.Should().Contain("\"puntos\":8");
        json.Should().Contain("\"puntos\":9.5");
    }

    [Fact(DisplayName = "CalificacionDto: deserializa con porcentaje opcional")]
    public void Calificacion_Deserialize_WithPorcentaje()
    {
        const string json = """
        {
            "calificacionID": 1,
            "inscripcionID": 2,
            "tipoEvaluacion": "Final",
            "puntos": 8.5,
            "puntosMaximos": 10,
            "porcentaje": 85.0
        }
        """;

        var c = JsonSerializer.Deserialize<CalificacionDto>(json)!;
        c.CalificacionID.Should().Be(1);
        c.Puntos.Should().Be(8.5m);
        c.Porcentaje.Should().Be(85.0m);
    }
}
