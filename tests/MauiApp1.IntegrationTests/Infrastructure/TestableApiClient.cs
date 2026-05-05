using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MauiApp1.src.Core.Models;

namespace MauiApp1.IntegrationTests.Infrastructure;

/// <summary>
/// Clon testeable del <c>ApiService</c> de MauiApp1. Es idéntico en los verbos
/// y rutas que consume, pero:
///   · Recibe <see cref="HttpClient"/> por DI para que podamos inyectar un
///     <see cref="HttpMessageHandler"/> stub desde los tests.
///   · No depende de <c>Microsoft.Maui.Storage.Preferences</c> para el JWT —
///     en su lugar expone <see cref="SetBearerToken"/> (equivalente funcional).
///
/// Cualquier cambio futuro en los endpoints del ApiService debería reflejarse
/// aquí (es el "espejo" que mantenemos en la suite de integración).
/// </summary>
public class TestableApiClient
{
    private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };
    private readonly HttpClient _http;

    public TestableApiClient(HttpClient http)
    {
        _http = http;
        if (_http.BaseAddress is null)
            _http.BaseAddress = new Uri("https://schoolmuai-api.onrender.com/");
    }

    public void SetBearerToken(string? token) =>
        _http.DefaultRequestHeaders.Authorization = string.IsNullOrEmpty(token)
            ? null : new AuthenticationHeaderValue("Bearer", token);

    // ---- AUTH ----
    public async Task<LoginResponse?> LoginUser(LoginRequest req)
    {
        var r = await _http.PostAsJsonAsync("api/auth/login", req);
        if (!r.IsSuccessStatusCode) return null;
        return await r.Content.ReadFromJsonAsync<LoginResponse>(_json);
    }

    public async Task<bool> RegisterUser(RegisterRequest data)
    {
        var r = await _http.PostAsJsonAsync("api/auth/register", data);
        return r.IsSuccessStatusCode;
    }

    // ---- ALUMNOS / MATERIAS / GRUPOS / DOCENTES ----
    public async Task<List<Alumno>> GetAlumnos()
    {
        var r = await _http.GetAsync("api/alumnos");
        if (!r.IsSuccessStatusCode) return new();
        return (await r.Content.ReadFromJsonAsync<List<Alumno>>(_json)) ?? new();
    }

    public async Task<List<Materia>> GetMaterias()
    {
        var r = await _http.GetAsync("api/materias");
        if (!r.IsSuccessStatusCode) return new();
        return (await r.Content.ReadFromJsonAsync<List<Materia>>(_json)) ?? new();
    }

    public async Task<List<Grupo>> GetGrupos()
    {
        var r = await _http.GetAsync("api/grupos");
        if (!r.IsSuccessStatusCode) return new();
        return (await r.Content.ReadFromJsonAsync<List<Grupo>>(_json)) ?? new();
    }

    public async Task<List<Grupo>> GetGruposDocente(int docenteID)
    {
        var r = await _http.GetAsync($"api/grupos/docente/{docenteID}");
        if (!r.IsSuccessStatusCode) return new();
        return (await r.Content.ReadFromJsonAsync<List<Grupo>>(_json)) ?? new();
    }

    public async Task<List<InscripcionDto>> GetInscripcionesGrupo(int grupoID)
    {
        var r = await _http.GetAsync($"api/grupos/{grupoID}/inscripciones");
        if (!r.IsSuccessStatusCode) return new();
        return (await r.Content.ReadFromJsonAsync<List<InscripcionDto>>(_json)) ?? new();
    }

    public async Task<List<DocenteDto>> GetDocentes()
    {
        var r = await _http.GetAsync("api/docentes");
        if (!r.IsSuccessStatusCode) return new();
        return (await r.Content.ReadFromJsonAsync<List<DocenteDto>>(_json)) ?? new();
    }

    // ---- QR ----
    public async Task<GenerarQrResponse?> GenerarQR(int sesionClaseID, int? duracionMinutos = null)
    {
        var r = await _http.PostAsJsonAsync("api/qr/generar", new GenerarQrRequest(sesionClaseID, duracionMinutos));
        if (!r.IsSuccessStatusCode) return null;
        return await r.Content.ReadFromJsonAsync<GenerarQrResponse>(_json);
    }

    public async Task<ValidarQrResponse?> ValidarQR(string token, int estudianteID,
        decimal? lat = null, decimal? lng = null)
    {
        var r = await _http.PostAsJsonAsync("api/qr/validar", new ValidarQrRequest(token, estudianteID, lat, lng));
        if (!r.IsSuccessStatusCode) return null;
        return await r.Content.ReadFromJsonAsync<ValidarQrResponse>(_json);
    }

    // ---- KÁRDEX / HOME / HORARIOS ----
    public async Task<List<KardexItemDto>> GetKardexEstudiante(int id)
    {
        var r = await _http.GetAsync($"api/kardex/estudiante/{id}");
        if (!r.IsSuccessStatusCode) return new();
        return (await r.Content.ReadFromJsonAsync<List<KardexItemDto>>(_json)) ?? new();
    }

    public async Task<HomeAlumnoDto?> GetHomeAlumno(int id)
    {
        var r = await _http.GetAsync($"api/estudiantes/{id}/home");
        if (!r.IsSuccessStatusCode) return null;
        return await r.Content.ReadFromJsonAsync<HomeAlumnoDto>(_json);
    }

    public async Task<List<HorarioDto>> GetHorarioEstudiante(int id)
    {
        var r = await _http.GetAsync($"api/horarios/estudiante/{id}");
        if (!r.IsSuccessStatusCode) return new();
        return (await r.Content.ReadFromJsonAsync<List<HorarioDto>>(_json)) ?? new();
    }

    public async Task<List<HorarioDto>> GetHorarioDocente(int id)
    {
        var r = await _http.GetAsync($"api/horarios/docente/{id}");
        if (!r.IsSuccessStatusCode) return new();
        return (await r.Content.ReadFromJsonAsync<List<HorarioDto>>(_json)) ?? new();
    }

    public async Task<List<ProximaAsistenciaDto>> GetProximasAsistencias(int id, int limit = 10)
    {
        var r = await _http.GetAsync($"api/asistencias/estudiante/{id}/proximas?limit={limit}");
        if (!r.IsSuccessStatusCode) return new();
        return (await r.Content.ReadFromJsonAsync<List<ProximaAsistenciaDto>>(_json)) ?? new();
    }

    // ---- CALIFICACIONES / ADMIN / AUDITORÍA ----
    public async Task<bool> CrearCalificacionesBulk(CalificacionBulkDto bulk)
    {
        var r = await _http.PostAsJsonAsync("api/calificaciones/bulk", bulk);
        return r.IsSuccessStatusCode;
    }

    public async Task<ImportResultDto?> ImportarUsuarios(ImportUsuariosRequest req)
    {
        var r = await _http.PostAsJsonAsync("api/admin/import", req);
        try { return await r.Content.ReadFromJsonAsync<ImportResultDto>(_json); }
        catch { return null; }
    }

    public async Task<List<EventoAuditoriaDto>> GetAuditoria(string tipo = "Todos",
        DateTime? desde = null, DateTime? hasta = null, int limit = 100)
    {
        var qs = new List<string> { $"tipo={Uri.EscapeDataString(tipo)}", $"limit={limit}" };
        if (desde.HasValue) qs.Add($"desde={Uri.EscapeDataString(desde.Value.ToString("o"))}");
        if (hasta.HasValue) qs.Add($"hasta={Uri.EscapeDataString(hasta.Value.ToString("o"))}");
        var r = await _http.GetAsync("api/auditoria?" + string.Join("&", qs));
        if (!r.IsSuccessStatusCode) return new();
        return (await r.Content.ReadFromJsonAsync<List<EventoAuditoriaDto>>(_json)) ?? new();
    }
}
