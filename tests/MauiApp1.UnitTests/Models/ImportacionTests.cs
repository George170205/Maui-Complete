using System.Text.Json;

namespace MauiApp1.UnitTests.Models;

/// <summary>
/// Pruebas del flujo de importación masiva de usuarios (Admin).
/// </summary>
public class ImportacionTests
{
    [Fact(DisplayName = "ImportUsuarioRow: record acepta telefono y password opcionales")]
    public void ImportRow_Defaults()
    {
        var r = new ImportUsuarioRow("a@b.com", "Juan", "Perez", 1);
        r.Telefono.Should().BeNull();
        r.PasswordInicial.Should().BeNull();
        r.RolID.Should().Be(1);
    }

    [Theory(DisplayName = "ImportUsuarioRow: admite los 3 roles válidos")]
    [InlineData(1)][InlineData(2)][InlineData(3)]
    public void ImportRow_Roles(int r)
    {
        var row = new ImportUsuarioRow("x@y", "N", "A", r);
        row.RolID.Should().BeInRange(1, 3);
    }

    [Fact(DisplayName = "ImportResultDto: defaults sanos")]
    public void ImportResult_Defaults()
    {
        var r = new ImportResultDto();
        r.Creados.Should().Be(0);
        r.Omitidos.Should().Be(0);
        r.Errores.Should().NotBeNull().And.BeEmpty();
    }

    [Fact(DisplayName = "ImportResultDto: deserializa resultado con errores")]
    public void ImportResult_DeserializesWithErrors()
    {
        const string json = """
        {"creados": 10, "omitidos": 2, "errores": ["Fila 3: email duplicado", "Fila 7: rol inválido"]}
        """;
        var r = JsonSerializer.Deserialize<ImportResultDto>(json)!;
        r.Creados.Should().Be(10);
        r.Omitidos.Should().Be(2);
        r.Errores.Should().HaveCount(2).And.Contain(s => s.Contains("duplicado"));
    }
}
