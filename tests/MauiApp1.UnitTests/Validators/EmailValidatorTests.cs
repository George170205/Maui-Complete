using System.Text.RegularExpressions;

namespace MauiApp1.UnitTests.Validators;

/// <summary>
/// Pruebas sobre reglas de validación de entrada usadas en LoginPage y RegistroPage.
/// (Regex básico de email + reglas de password mínimo 6 caracteres).
/// </summary>
public class EmailValidatorTests
{
    // Mismo patrón que suele usar la LoginPage.
    private static readonly Regex EmailRegex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled);

    [Theory(DisplayName = "Email: acepta formatos válidos")]
    [InlineData("a@b.com")]
    [InlineData("alumno@escuela.edu.mx")]
    [InlineData("nombre.apellido+tag@dominio.co")]
    public void Email_Valid(string email) => EmailRegex.IsMatch(email).Should().BeTrue();

    [Theory(DisplayName = "Email: rechaza formatos inválidos")]
    [InlineData("")]
    [InlineData("sin-arroba")]
    [InlineData("@sin-local.com")]
    [InlineData("sin@dominio")]
    [InlineData("espacios en @ email.com")]
    public void Email_Invalid(string email) => EmailRegex.IsMatch(email).Should().BeFalse();

    [Theory(DisplayName = "Password: mínimo 6 chars")]
    [InlineData("12345", false)]
    [InlineData("abcdef", true)]
    [InlineData("Secreto1!", true)]
    public void Password_MinLength(string pwd, bool valid)
    {
        (pwd.Length >= 6).Should().Be(valid);
    }
}
