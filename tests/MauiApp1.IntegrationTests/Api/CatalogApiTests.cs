namespace MauiApp1.IntegrationTests.Api;

/// <summary>
/// Integración — endpoints de catálogo (materias, grupos, alumnos, docentes).
/// </summary>
public class CatalogApiTests
{
    [Fact(DisplayName = "GetMaterias: 200 → lista parseada")]
    public async Task GetMaterias_Ok()
    {
        const string body = """
        [
          {"MateriaID":1,"codigo":"MAT","nombre":"Álgebra","creditos":6,"activo":true},
          {"MateriaID":2,"codigo":"FIS","nombre":"Física","creditos":5,"activo":true}
        ]
        """;
        var api = new TestableApiClient(new HttpClient(StubHttpMessageHandler.ReturnsJson(HttpStatusCode.OK, body)));
        var l = await api.GetMaterias();
        l.Should().HaveCount(2);
        l[0].Nombre.Should().Be("Álgebra");
        l[1].Codigo.Should().Be("FIS");
    }

    [Fact(DisplayName = "GetMaterias: 500 → lista vacía (no null)")]
    public async Task GetMaterias_Error()
    {
        var api = new TestableApiClient(new HttpClient(StubHttpMessageHandler.ReturnsStatus(HttpStatusCode.InternalServerError)));
        (await api.GetMaterias()).Should().NotBeNull().And.BeEmpty();
    }

    [Fact(DisplayName = "GetAlumnos: deserializa colección")]
    public async Task GetAlumnos_Ok()
    {
        const string body = """
        [{"AlumnoID":1,"Nombre":"Juan","Apellido":"Pérez","Matricula":"A1","Email":"a@b","Activo":true,"UsuarioID":11}]
        """;
        var api = new TestableApiClient(new HttpClient(StubHttpMessageHandler.ReturnsJson(HttpStatusCode.OK, body)));
        var l = await api.GetAlumnos();
        l.Should().HaveCount(1);
        l[0].Matricula.Should().Be("A1");
    }

    [Fact(DisplayName = "GetGrupos: lista con Materia anidada")]
    public async Task GetGrupos_Ok()
    {
        const string body = """
        [{"grupoID":1,"materiaID":10,"nombre":"G-A","materia":{"MateriaID":10,"nombre":"BD"}}]
        """;
        var api = new TestableApiClient(new HttpClient(StubHttpMessageHandler.ReturnsJson(HttpStatusCode.OK, body)));
        var l = await api.GetGrupos();
        l.Should().HaveCount(1);
        l[0].Materia!.Nombre.Should().Be("BD");
    }

    [Fact(DisplayName = "GetGruposDocente: URL incluye docenteID")]
    public async Task GetGruposDocente_UrlOk()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        { Content = new StringContent("[]", System.Text.Encoding.UTF8, "application/json") });
        var api = new TestableApiClient(new HttpClient(handler));
        await api.GetGruposDocente(77);
        handler.Requests[0].RequestUri!.ToString().Should().EndWith("api/grupos/docente/77");
    }

    [Fact(DisplayName = "GetInscripcionesGrupo: URL y parseo de lista")]
    public async Task GetInscripciones_Ok()
    {
        const string body = """[{"inscripcionID":1,"estudianteID":33,"nombre":"Juan","matricula":"A1","calificacionFinal":9.0,"estado":"Aprobado"}]""";
        var handler = StubHttpMessageHandler.ReturnsJson(HttpStatusCode.OK, body);
        var api = new TestableApiClient(new HttpClient(handler));
        var l = await api.GetInscripcionesGrupo(12);
        l.Should().HaveCount(1);
        l[0].CalificacionFinal.Should().Be(9.0m);
        handler.Requests[0].RequestUri!.ToString().Should().EndWith("api/grupos/12/inscripciones");
    }

    [Fact(DisplayName = "GetDocentes: deserializa gruposAsignados")]
    public async Task GetDocentes_Ok()
    {
        const string body = """[{"docenteID":1,"nombre":"Ana","apellido":"Ruiz","email":"a@u","gruposAsignados":4}]""";
        var api = new TestableApiClient(new HttpClient(StubHttpMessageHandler.ReturnsJson(HttpStatusCode.OK, body)));
        var l = await api.GetDocentes();
        l[0].GruposAsignados.Should().Be(4);
    }
}
