namespace MauiApp1.UnitTests.Models;

/// <summary>
/// Pruebas unitarias del modelo <c>Alumno</c>.
/// Verifica la inicialización de propiedades, valores por defecto y mutabilidad.
/// </summary>
public class AlumnoTests
{
    [Fact(DisplayName = "Alumno: nuevas instancias tienen valores por defecto correctos")]
    public void NewAlumno_HasDefaultValues()
    {
        var a = new Alumno();
        a.AlumnoID.Should().Be(0);
        a.UsuarioID.Should().Be(0);
        a.Activo.Should().BeFalse();
        a.Nombre.Should().BeNull();
        a.Apellido.Should().BeNull();
        a.Matricula.Should().BeNull();
        a.Email.Should().BeNull();
        a.Telefono.Should().BeNull();
    }

    [Fact(DisplayName = "Alumno: propiedades se asignan correctamente")]
    public void Alumno_PropertiesAssign_WorksCorrectly()
    {
        var a = new Alumno
        {
            AlumnoID = 7,
            UsuarioID = 42,
            Nombre = "Juan",
            Apellido = "Pérez",
            Matricula = "A1234",
            Email = "juan@test.com",
            Telefono = "5551234",
            Activo = true
        };

        a.AlumnoID.Should().Be(7);
        a.UsuarioID.Should().Be(42);
        a.Nombre.Should().Be("Juan");
        a.Apellido.Should().Be("Pérez");
        a.Matricula.Should().Be("A1234");
        a.Email.Should().Be("juan@test.com");
        a.Telefono.Should().Be("5551234");
        a.Activo.Should().BeTrue();
    }

    [Theory(DisplayName = "Alumno: acepta IDs positivos arbitrarios")]
    [InlineData(1)]
    [InlineData(999)]
    [InlineData(int.MaxValue)]
    public void Alumno_AcceptsPositiveIds(int id)
    {
        var a = new Alumno { AlumnoID = id };
        a.AlumnoID.Should().Be(id);
    }
}
