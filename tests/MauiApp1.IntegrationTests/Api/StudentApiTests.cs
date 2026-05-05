namespace MauiApp1.IntegrationTests.Api;

/// <summary>
/// Integración — endpoints del estudiante: home, kárdex, horario, próximas asistencias.
/// </summary>
public class StudentApiTests
{
    [Fact(DisplayName = "GetHomeAlumno: parsea estructura agregada")]
    public async Task GetHomeAlumno_Ok()
    {
        const string body = """
        {"estudianteID":33,"nombreCompleto":"Juan Pérez","iniciales":"JP","matricula":"A1",
         "carrera":"ISC","semestre":5,"email":"j@t.com","materiasActivas":6,"promedio":8.5,
         "porcentajeAsistencia":92,"faltasTotales":3,
         "proximaClase":{"sesionClaseID":1,"grupoID":2,"nombre":"BD","fecha":"2026-04-21T09:00:00Z",
                         "horaInicio":"09:00","horaFin":"10:30"},
         "materiasList":[{"materiaID":1,"grupoID":1,"nombre":"BD","codigo":"BD1","porcentaje":90}]}
        """;
        var handler = StubHttpMessageHandler.ReturnsJson(HttpStatusCode.OK, body);
        var api = new TestableApiClient(new HttpClient(handler));
        var h = await api.GetHomeAlumno(33);
        h.Should().NotBeNull();
        h!.Matricula.Should().Be("A1");
        h.ProximaClase!.Nombre.Should().Be("BD");
        handler.Requests[0].RequestUri!.ToString().Should().EndWith("api/estudiantes/33/home");
    }

    [Fact(DisplayName = "GetHomeAlumno: 404 → null")]
    public async Task GetHomeAlumno_NotFound()
    {
        var api = new TestableApiClient(new HttpClient(StubHttpMessageHandler.ReturnsStatus(HttpStatusCode.NotFound)));
        (await api.GetHomeAlumno(999)).Should().BeNull();
    }

    [Fact(DisplayName = "GetKardex: lista con % asistencia")]
    public async Task GetKardex_Ok()
    {
        const string body = """
        [{"inscripcionID":1,"materiaID":5,"nombre":"Álgebra","calificacionFinal":9.0,
          "sesionesTotales":40,"sesionesAsistidas":38,"porcentajeAsistencia":95.0,"estado":"Aprobado"}]
        """;
        var api = new TestableApiClient(new HttpClient(StubHttpMessageHandler.ReturnsJson(HttpStatusCode.OK, body)));
        var k = await api.GetKardexEstudiante(33);
        k.Should().HaveCount(1);
        k[0].PorcentajeAsistencia.Should().Be(95.0m);
    }

    [Fact(DisplayName = "GetHorarioEstudiante: ruta y lista")]
    public async Task GetHorarioEstudiante_Ok()
    {
        const string body = """[{"horarioID":1,"diaSemana":1,"horaInicio":"08:00","horaFin":"10:00","materiaNombre":"BD"}]""";
        var handler = StubHttpMessageHandler.ReturnsJson(HttpStatusCode.OK, body);
        var api = new TestableApiClient(new HttpClient(handler));
        var l = await api.GetHorarioEstudiante(33);
        l.Should().HaveCount(1);
        handler.Requests[0].RequestUri!.ToString().Should().EndWith("api/horarios/estudiante/33");
    }

    [Fact(DisplayName = "GetProximasAsistencias: usa query param limit")]
    public async Task GetProximas_UsesLimit()
    {
        var handler = StubHttpMessageHandler.ReturnsJson(HttpStatusCode.OK, "[]");
        var api = new TestableApiClient(new HttpClient(handler));
        await api.GetProximasAsistencias(33, limit: 5);
        handler.Requests[0].RequestUri!.Query.Should().Contain("limit=5");
    }
}
