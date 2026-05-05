namespace MauiApp1.IntegrationTests.Api;

/// <summary>
/// Integración — endpoints /api/qr/generar y /api/qr/validar (PDF §3.3).
/// </summary>
public class QrApiTests
{
    [Fact(DisplayName = "GenerarQR: 200 → devuelve token + imagen base64")]
    public async Task GenerarQR_Ok()
    {
        const string body = """
        {"QrGeneradoID":1,"Token":"TOK-123","ExpiraEn":"2026-04-20T12:00:00Z","DuracionMinutos":10,"ImagenBase64":"data:image/png;base64,AAAA"}
        """;
        var api = new TestableApiClient(new HttpClient(StubHttpMessageHandler.ReturnsJson(HttpStatusCode.OK, body)));
        var r = await api.GenerarQR(50, 10);
        r.Should().NotBeNull();
        r!.Token.Should().Be("TOK-123");
        r.ImagenBase64.Should().StartWith("data:image/png;base64,");
    }

    [Fact(DisplayName = "GenerarQR: 403 sin auth → null")]
    public async Task GenerarQR_Forbidden()
    {
        var api = new TestableApiClient(new HttpClient(StubHttpMessageHandler.ReturnsStatus(HttpStatusCode.Forbidden)));
        (await api.GenerarQR(1)).Should().BeNull();
    }

    [Fact(DisplayName = "ValidarQR: 200 → mensaje de asistencia registrada")]
    public async Task ValidarQR_Ok()
    {
        const string body = """{"Message":"Asistencia registrada","AsistenciaID":42}""";
        var api = new TestableApiClient(new HttpClient(StubHttpMessageHandler.ReturnsJson(HttpStatusCode.OK, body)));
        var r = await api.ValidarQR("TOK-123", 33);
        r.Should().NotBeNull();
        r!.AsistenciaID.Should().Be(42);
    }

    [Fact(DisplayName = "ValidarQR: 400 token expirado/inválido → null")]
    public async Task ValidarQR_Invalid()
    {
        var api = new TestableApiClient(new HttpClient(StubHttpMessageHandler.ReturnsStatus(HttpStatusCode.BadRequest)));
        (await api.ValidarQR("bad", 33)).Should().BeNull();
    }

    [Fact(DisplayName = "ValidarQR: incluye lat/long en body cuando se provee")]
    public async Task ValidarQR_SendsCoords()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        { Content = new StringContent("{\"Message\":\"ok\"}", System.Text.Encoding.UTF8, "application/json") });
        var api = new TestableApiClient(new HttpClient(handler));
        await api.ValidarQR("t", 7, 19.43m, -99.13m);
        var body = await handler.Requests[0].Content!.ReadAsStringAsync();
        body.Should().Contain("19.43").And.Contain("-99.13");
    }
}
