namespace MauiApp1.IntegrationTests.Api;

/// <summary>
/// Integración — endpoints /api/auth/*.
/// Usa HttpClient con handler stub para ser determinista.
/// </summary>
public class AuthApiTests
{
    [Fact(DisplayName = "Login: 200 → devuelve LoginResponse poblado")]
    public async Task Login_200_ReturnsResponse()
    {
        const string body = """
        {"token":"abc.def.ghi","rol":"Estudiante","rolID":1,"estudianteID":33,"email":"a@b.com","nombre":"Juan"}
        """;
        var http = new HttpClient(StubHttpMessageHandler.ReturnsJson(HttpStatusCode.OK, body));
        var api = new TestableApiClient(http);

        var r = await api.LoginUser(new LoginRequest { Email = "a@b.com", Password = "123456" });

        r.Should().NotBeNull();
        r!.Token.Should().Be("abc.def.ghi");
        r.Rol.Should().Be("Estudiante");
        r.EstudianteID.Should().Be(33);
    }

    [Fact(DisplayName = "Login: 401 → devuelve null (credenciales inválidas)")]
    public async Task Login_401_ReturnsNull()
    {
        var http = new HttpClient(StubHttpMessageHandler.ReturnsStatus(HttpStatusCode.Unauthorized));
        var api = new TestableApiClient(http);
        (await api.LoginUser(new LoginRequest { Email = "x@y", Password = "bad" })).Should().BeNull();
    }

    [Fact(DisplayName = "Login: envía body JSON con email y password")]
    public async Task Login_BodyIsCorrect()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Unauthorized));
        var api = new TestableApiClient(new HttpClient(handler));
        await api.LoginUser(new LoginRequest { Email = "foo@bar.com", Password = "topsecret" });

        handler.Requests.Should().ContainSingle();
        var body = await handler.Requests[0].Content!.ReadAsStringAsync();
        body.Should().Contain("foo@bar.com").And.Contain("topsecret");
        handler.Requests[0].RequestUri!.ToString().Should().EndWith("api/auth/login");
        handler.Requests[0].Method.Should().Be(HttpMethod.Post);
    }

    [Fact(DisplayName = "Register: 201 → true")]
    public async Task Register_Ok()
    {
        var http = new HttpClient(StubHttpMessageHandler.ReturnsStatus(HttpStatusCode.Created));
        var api = new TestableApiClient(http);
        (await api.RegisterUser(new RegisterRequest { Email = "nuevo@x", Password = "abcdef", Nombre = "N", Apellido = "A", RolID = 1 }))
            .Should().BeTrue();
    }

    [Fact(DisplayName = "Register: 400 → false")]
    public async Task Register_BadRequest()
    {
        var http = new HttpClient(StubHttpMessageHandler.ReturnsStatus(HttpStatusCode.BadRequest));
        var api = new TestableApiClient(http);
        (await api.RegisterUser(new RegisterRequest())).Should().BeFalse();
    }

    [Fact(DisplayName = "SetBearerToken: inyecta header Authorization en siguientes requests")]
    public async Task SetBearerToken_AttachesHeader()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NoContent));
        var api = new TestableApiClient(new HttpClient(handler));
        api.SetBearerToken("my-jwt");

        await api.GetAlumnos();

        var auth = handler.Requests[0].Headers.Authorization;
        auth.Should().NotBeNull();
        auth!.Scheme.Should().Be("Bearer");
        auth.Parameter.Should().Be("my-jwt");
    }
}
