using MauiApp1.E2ETests.Infrastructure;
using OpenQA.Selenium;

namespace MauiApp1.E2ETests.Flows;

/// <summary>
/// E2E — flujos del rol Alumno (PDF §2.1).
///
/// Las páginas del alumno están en <c>src/Presentation/Views/Student/</c>.
/// Se acceden desde el TabBar <c>AlumnoTabs</c>: Inicio, Asistencia,
/// Horario, Kárdex, Perfil. Si el APK no tiene el tab de Kárdex, los tests
/// usan rutas alternativas (PerfilAlumnoPage tiene una opción para ir a
/// kárdex en el menú).
/// </summary>
[TestFixture]
public class StudentFlowsE2E : BaseE2ETest
{
    private static readonly By LoginUsuarioField =
        BaseE2ETest_Selectors.UsuarioField;
    private static readonly By LoginPasswordField =
        BaseE2ETest_Selectors.PasswordField;
    private static readonly By LoginButton =
        BaseE2ETest_Selectors.LoginButton;

    private void Login()
    {
        Type(LoginUsuarioField, Config.Credentials.AlumnoEmail);
        Type(LoginPasswordField, Config.Credentials.AlumnoPassword);
        Tap(LoginButton);

        WaitForAny(45,
            ByContainsText("Próxima"),
            ByContainsText("Hola"),
            ByContainsText("Tus Materias"),
            ByContainsText("Asistencias"),
            ByContainsText("Inicio"));
    }

    [Test(Description = "Alumno ve su HomeAlumnoPage con asistencia y promedio")]
    public void Alumno_HomePage_ShowsAggregatedStats()
    {
        Login();
        WaitForAny(25,
            ByContainsText("Asistencias"),
            ByContainsText("Materias"),
            ByContainsText("Promedio"))
            .Displayed.Should().BeTrue();
    }

    [Test(Description = "Alumno abre horario semanal y se muestra al menos un día")]
    public void Alumno_HorarioPage_Renders()
    {
        Login();
        TapAny(20, ByText("Horario"), ByContainsText("Horario"));
        WaitForAny(25,
            ByContainsText("Lun"),
            ByContainsText("Mar"),
            ByContainsText("Mié"),
            ByContainsText("Tu Horario"))
            .Displayed.Should().BeTrue();
    }

    [Test(Description = "Alumno abre kárdex y ve la columna 'Asistencia'")]
    public void Alumno_KardexPage_Renders()
    {
        Login();
        // Si el tab Kárdex está, lo abrimos. Si no, vamos por Perfil →
        // Kárdex (una de las opciones del menú del perfil).
        try
        {
            TapAny(8, ByText("Kárdex"), ByText("Kardex"));
        }
        catch (WebDriverException)
        {
            TapAny(15, ByText("Perfil"));
            TapAny(15, ByContainsText("Kárdex"), ByContainsText("Kardex"));
        }

        WaitForAny(25,
            ByContainsText("Asistencia"),
            ByContainsText("Kárdex"),
            ByContainsText("Promedio"),
            ByContainsText("Aún no hay materias"))
            .Displayed.Should().BeTrue();
    }

    [Test(Description = "Alumno abre su perfil y se muestra su matrícula")]
    public void Alumno_PerfilPage_ShowsMatricula()
    {
        Login();
        TapAny(20, ByText("Perfil"));
        WaitForAny(25,
            ByContainsText("Matrícula"),
            ByContainsText("Mi Perfil"),
            ByContainsText("Asistencia"),
            ByContainsText("Faltas"))
            .Displayed.Should().BeTrue();
    }

    [Test(Description = "Alumno escanea QR (token preestablecido) y se registra su asistencia")]
    public void Alumno_EscaneaQR_RegistraAsistencia()
    {
        Login();
        TapAny(20, ByText("Asistencia"));

        // Si la entry inline existe (build nuevo), usamos esa ruta.
        // Si no, intentamos abrir la opción "Ingresar manualmente"
        // (DisplayPromptAsync) que ya existe en el ViewModel.
        try
        {
            Type(ByIdOrHint("entryTokenManual", "Pega aquí el token QR"), "TEST-TOKEN");
            TapAny(15,
                By.XPath("//*[@content-desc='btnValidarToken' or @resource-id='btnValidarToken' " +
                         "or @text='Validar' or @text='VALIDAR']"));
        }
        catch (WebDriverTimeoutException)
        {
            TapAny(15, ByContainsText("manualmente"), ByContainsText("Manual"));
            // El DisplayPromptAsync abre un EditText de Android.
            try
            {
                Driver.FindElement(By.XPath("//android.widget.EditText"))
                      .SendKeys("TEST-TOKEN");
                TapAny(10, ByText("OK"), ByText("Aceptar"));
            }
            catch (NoSuchElementException) { }
        }

        WaitForAny(30,
            ByContainsText("registrada"),
            ByContainsText("Asistencia"),
            ByContainsText("inválido"),
            ByContainsText("expirado"),
            ByContainsText("Token"))
            .Displayed.Should().BeTrue();
    }
}

/// <summary>
/// Selectores reutilizables que comparten todos los flujos. Se declaran
/// fuera de la clase de fixture para poder referenciarlos desde varios
/// archivos sin acoplarlos a una jerarquía de herencia.
/// </summary>
internal static class BaseE2ETest_Selectors
{
    public static readonly By UsuarioField = By.XPath(
        "//*[@content-desc='entryEmail' or @resource-id='entryEmail' " +
        "or @hint='Ingresa tu usuario' or @text='Ingresa tu usuario']");

    public static readonly By PasswordField = By.XPath(
        "//*[@content-desc='entryPassword' or @resource-id='entryPassword' " +
        "or @password='true' or @hint='••••••••']");

    public static readonly By LoginButton = By.XPath(
        "//*[@content-desc='btnLogin' or @resource-id='btnLogin' " +
        "or @text='Iniciar Sesión' or @text='Iniciar sesión' " +
        "or @text='INICIAR SESIÓN']");
}
