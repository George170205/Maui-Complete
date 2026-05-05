namespace MauiApp1.IntegrationTests.Api;

/// <summary>
/// Integración — endpoints del docente (bulk calificaciones) y del admin
/// (importación masiva, auditoría).
/// </summary>
public class TeacherAdminApiTests
{
    [Fact(DisplayName = "CrearCalificacionesBulk: 200 → true")]
    public async Task Bulk_Ok()
    {
        var api = new TestableApiClient(new HttpClient(StubHttpMessageHandler.ReturnsStatus(HttpStatusCode.OK)));
        var dto = new CalificacionBulkDto
        {
            Items = new() { new() { InscripcionID = 1, Puntos = 8 }, new() { InscripcionID = 2, Puntos = 9 } }
        };
        (await api.CrearCalificacionesBulk(dto)).Should().BeTrue();
    }

    [Fact(DisplayName = "CrearCalificacionesBulk: 500 → false (tx abortada)")]
    public async Task Bulk_Error()
    {
        var api = new TestableApiClient(new HttpClient(StubHttpMessageHandler.ReturnsStatus(HttpStatusCode.InternalServerError)));
        (await api.CrearCalificacionesBulk(new CalificacionBulkDto())).Should().BeFalse();
    }

    [Fact(DisplayName = "ImportarUsuarios: 200 → DTO con creados/omitidos")]
    public async Task Import_Ok()
    {
        const string body = """{"creados":10,"omitidos":2,"errores":[]}""";
        var api = new TestableApiClient(new HttpClient(StubHttpMessageHandler.ReturnsJson(HttpStatusCode.OK, body)));
        var r = await api.ImportarUsuarios(new ImportUsuariosRequest
        {
            Filas = new() { new("a@x", "A", "B", 1) }
        });
        r.Should().NotBeNull();
        r!.Creados.Should().Be(10);
        r.Omitidos.Should().Be(2);
    }

    [Fact(DisplayName = "ImportarUsuarios: 500 aún intenta parsear cuerpo con errores")]
    public async Task Import_ErrorWithBody()
    {
        const string body = """{"creados":0,"omitidos":0,"errores":["Fila 1: email inválido"]}""";
        var api = new TestableApiClient(new HttpClient(StubHttpMessageHandler.ReturnsJson(HttpStatusCode.InternalServerError, body)));
        var r = await api.ImportarUsuarios(new ImportUsuariosRequest());
        r.Should().NotBeNull();
        r!.Errores.Should().HaveCount(1);
    }

    [Fact(DisplayName = "GetAuditoria: arma query con tipo/desde/hasta/limit")]
    public async Task Auditoria_QueryBuilt()
    {
        var handler = StubHttpMessageHandler.ReturnsJson(HttpStatusCode.OK, "[]");
        var api = new TestableApiClient(new HttpClient(handler));
        await api.GetAuditoria("Login", new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc), 50);
        var q = handler.Requests[0].RequestUri!.Query;
        q.Should().Contain("tipo=Login");
        q.Should().Contain("limit=50");
        q.Should().Contain("desde=").And.Contain("hasta=");
    }

    [Fact(DisplayName = "GetAuditoria: deserializa eventos")]
    public async Task Auditoria_Deserializes()
    {
        const string body = """[{"tipo":"Login","fecha":"2026-04-20T10:00:00Z","resumen":"x@y logueado"}]""";
        var api = new TestableApiClient(new HttpClient(StubHttpMessageHandler.ReturnsJson(HttpStatusCode.OK, body)));
        var l = await api.GetAuditoria();
        l.Should().HaveCount(1);
        l[0].Tipo.Should().Be("Login");
    }
}
