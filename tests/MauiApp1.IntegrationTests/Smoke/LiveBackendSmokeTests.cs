namespace MauiApp1.IntegrationTests.Smoke;

/// <summary>
/// Smoke tests contra el backend real <c>schoolmuai-api.onrender.com</c>.
/// Se omiten salvo que la variable <c>RUN_LIVE_TESTS=1</c> esté seteada para
/// evitar flakiness en CI (cold-starts de Render, red, etc.).
///
/// Activar en local:
///   Windows (PowerShell): $env:RUN_LIVE_TESTS=1; dotnet test
///   Linux/macOS:          RUN_LIVE_TESTS=1 dotnet test
/// </summary>
public class LiveBackendSmokeTests
{
    private static bool Enabled =>
        string.Equals(Environment.GetEnvironmentVariable("RUN_LIVE_TESTS"), "1", StringComparison.Ordinal);

    private static HttpClient MakeClient() => new() { Timeout = TimeSpan.FromSeconds(45) };

    [SkippableFact(DisplayName = "Live: GET /api/materias responde 200 con JSON array")]
    public async Task Materias_Live_Ok()
    {
        Skip.IfNot(Enabled, "RUN_LIVE_TESTS no está definido");
        var api = new TestableApiClient(MakeClient());
        var l = await api.GetMaterias();
        l.Should().NotBeNull();
    }

    [SkippableFact(DisplayName = "Live: login con credenciales inválidas devuelve null (401)")]
    public async Task LoginFail_Live()
    {
        Skip.IfNot(Enabled, "RUN_LIVE_TESTS no está definido");
        var api = new TestableApiClient(MakeClient());
        var r = await api.LoginUser(new LoginRequest { Email = "no-existe@t.com", Password = "xxxxxx" });
        r.Should().BeNull();
    }
}

/// <summary>Marcador simple para simular Skip si no está SkippableFact.</summary>
public static class Skip
{
    public static void IfNot(bool condition, string reason)
    {
        if (!condition)
            throw new Xunit.Sdk.XunitException($"SKIP: {reason}");
    }
}

/// <summary>Atributo mínimo "SkippableFact" (FactAttribute). Si se quisiera
/// soporte nativo reemplazar con el paquete Xunit.SkippableFact.</summary>
public sealed class SkippableFactAttribute : FactAttribute
{
    public SkippableFactAttribute() { }
}
