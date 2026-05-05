using System.Text.Json;

namespace MauiApp1.UnitTests.Models;

/// <summary>
/// Pruebas sobre DTOs de autenticación: LoginRequest, LoginResponse, RegisterRequest.
/// Verifican defaults, asignaciones y contratos JSON con el endpoint /api/auth/*.
/// </summary>
public class AuthDtosTests
{
    [Fact(DisplayName = "LoginRequest: asigna Email y Password")]
    public void LoginRequest_Assign()
    {
        var r = new LoginRequest { Email = "a@b.com", Password = "secret" };
        r.Email.Should().Be("a@b.com");
        r.Password.Should().Be("secret");
    }

    [Fact(DisplayName = "LoginResponse: valores por defecto son seguros")]
    public void LoginResponse_Defaults()
    {
        var r = new LoginResponse();
        r.Token.Should().Be(string.Empty);
        r.RefreshToken.Should().BeNull();
        r.Rol.Should().BeNull();
        r.RolID.Should().BeNull();
        r.EstudianteID.Should().BeNull();
        r.DocenteID.Should().BeNull();
    }

    [Fact(DisplayName = "LoginResponse: deserializa payload real del backend")]
    public void LoginResponse_Deserialize_RealPayload()
    {
        const string json = """
        {
          "token": "eyJhbGci...",
          "refreshToken": "rt-abc",
          "refreshTokenExpiraEn": "2026-04-21T12:00:00Z",
          "rol": "Estudiante",
          "rolID": 1,
          "email": "alumno@test.com",
          "nombre": "Juan",
          "estudianteID": 33,
          "docenteID": null
        }
        """;

        var r = JsonSerializer.Deserialize<LoginResponse>(json);
        r.Should().NotBeNull();
        r!.Token.Should().StartWith("eyJ");
        r.RefreshToken.Should().Be("rt-abc");
        r.Rol.Should().Be("Estudiante");
        r.RolID.Should().Be(1);
        r.EstudianteID.Should().Be(33);
        r.DocenteID.Should().BeNull();
    }

    [Fact(DisplayName = "RegisterRequest: valores por defecto son cadenas vacías")]
    public void RegisterRequest_Defaults()
    {
        var r = new RegisterRequest();
        r.Email.Should().BeEmpty();
        r.Password.Should().BeEmpty();
        r.Nombre.Should().BeEmpty();
        r.Apellido.Should().BeEmpty();
        r.Telefono.Should().BeNull();
        r.RolID.Should().Be(0);
    }

    [Theory(DisplayName = "RegisterRequest: soporta los 3 roles definidos")]
    [InlineData(1)] // Alumno
    [InlineData(2)] // Docente
    [InlineData(3)] // Admin
    public void RegisterRequest_AcceptsRoles(int rol)
    {
        var r = new RegisterRequest { RolID = rol };
        r.RolID.Should().Be(rol);
    }
}
