using System.Text.Json;

namespace MauiApp1.UnitTests.Models;

/// <summary>
/// Pruebas de EventoAuditoriaDto (PDF §2.3).
/// </summary>
public class AuditoriaTests
{
    [Fact(DisplayName = "EventoAuditoriaDto: deserializa evento completo")]
    public void Evento_Deserialize()
    {
        const string json = """
        {"tipo":"Login","fecha":"2026-04-20T10:00:00Z","resumen":"Usuario X inició sesión","meta":"{\"ip\":\"1.1.1.1\"}"}
        """;

        var e = JsonSerializer.Deserialize<EventoAuditoriaDto>(json)!;
        e.Tipo.Should().Be("Login");
        e.Resumen.Should().Contain("inició sesión");
        e.Meta.Should().Contain("ip");
        e.Fecha.Year.Should().Be(2026);
    }

    [Fact(DisplayName = "EventoAuditoriaDto: Meta puede ser null")]
    public void Evento_Meta_Nullable()
    {
        var e = new EventoAuditoriaDto { Tipo = "X", Resumen = "y" };
        e.Meta.Should().BeNull();
    }
}
