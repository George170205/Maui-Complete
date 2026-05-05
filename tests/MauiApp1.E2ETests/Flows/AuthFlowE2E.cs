using MauiApp1.E2ETests.Infrastructure;
using OpenQA.Selenium;

namespace MauiApp1.E2ETests.Flows;

/// <summary>
/// E2E — flujo de autenticación completo (Login + Registro).
///
/// Selectores diseñados para tolerar dos escenarios:
///   1) El APK lleva los <c>AutomationId</c> nuevos (entryEmail, etc.).
///   2) MAUI no propaga AutomationId al árbol Android: caemos al
///      placeholder visible (<c>hint</c>) que ya estaba en el XAML
///      original.
/// </summary>
[TestFixture]
public class AuthFlowE2E : BaseE2ETest
{
    // Selectores reutilizables — combinan AutomationId + placeholder.
    private static readonly By LoginUsuarioField =
        ByIdOrHint("entryEmail", "Ingresa tu usuario");

    private static readonly By LoginPasswordField =
        // El placeholder son bullets ••••••••, lo intentamos también por
        // atributo password="true" en el XPath final.
        By.XPath("//*[@content-desc='entryPassword' or @resource-id='entryPassword' " +
                 "or @password='true' " +
                 "or @hint='••••••••']");

    private static readonly By LoginButton =
        By.XPath("//*[@content-desc='btnLogin' or @resource-id='btnLogin' " +
                 "or @text='Iniciar Sesión' or @text='Iniciar sesión' " +
                 "or @text='INICIAR SESIÓN' or @text='INICIAR SESIóN']");

    [Test(Description = "Login con credenciales válidas navega al Home del alumno")]
    public void Login_Alumno_NavigatesToHome()
    {
        Type(LoginUsuarioField, Config.Credentials.AlumnoEmail);
        Type(LoginPasswordField, Config.Credentials.AlumnoPassword);
        Tap(LoginButton);

        // HomeAlumnoPage muestra "Próxima clase" o el saludo "Hola,".
        // Si el backend no devolvió datos válidos, al menos verificamos
        // que dejamos la pantalla de login.
        WaitForAny(45,
            ByContainsText("Próxima"),
            ByContainsText("Hola"),
            ByContainsText("Tus Materias"),
            ByContainsText("Asistencias"),
            ByContainsText("Inicio"))
            .Displayed.Should().BeTrue();
    }

    [Test(Description = "Login con credenciales inválidas muestra error")]
    public void Login_Invalid_ShowsError()
    {
        Type(LoginUsuarioField, "no-existe@test.com");
        Type(LoginPasswordField, "wrong-pass");
        Tap(LoginButton);

        // El code-behind hace DisplayAlert("Error","Credenciales incorrectas").
        WaitForAny(25,
            ByContainsText("Credenciales"),
            ByContainsText("incorrect"),
            ByText("Error"),
            ByContainsText("Completa"))
            .Displayed.Should().BeTrue();
    }

    [Test(Description = "Registro de nuevo usuario — flujo feliz hasta banner de éxito")]
    public void Registro_NuevoUsuario_Flujo()
    {
        // Desde LoginPage: la etiqueta "Regístrate aquí" navega a RegistroPage.
        TapAny(20,
            ByContainsText("Regístrate"),
            ByContainsText("Registrarse"),
            ByContainsText("Crear cuenta"),
            ByContainsText("Crear Cuenta"));

        var unique = $"qa.{DateTime.UtcNow.Ticks}@test.com";

        // Selección de rol — el Picker MAUI en Android se muestra con su
        // Title ("Seleccionar rol"). Lo abrimos y elegimos Alumno.
        TapAny(15,
            ByIdOrHint("pickerRol", "Seleccionar rol"),
            ByContainsText("Seleccionar rol"));

        // El item del Picker en MAUI Android aparece como TextView en el
        // diálogo. Probamos con y sin espacio inicial (el XAML del Picker
        // de Login tiene espacios; el de Registro no).
        TapAny(10, ByText("Alumno"), ByText(" Alumno"));
        // Algunos diálogos requieren OK explícito.
        try
        {
            Driver.FindElement(By.XPath("//*[@text='OK' or @text='Aceptar']")).Click();
        }
        catch (NoSuchElementException) { /* el picker se cerró solo */ }
        catch (WebDriverException) { /* idem */ }

        Type(ByIdOrHint("entryEmail", "ejemplo@correo.com"), unique);
        Type(ByIdOrHint("entryNombre", "Tu nombre"), "QA");
        Type(ByIdOrHint("entryApellido", "Tu apellido"), "User");

        // Para passwords usamos atributo password=true como respaldo —
        // hay 2 campos password (passwordEntry y confirmarPasswordEntry).
        var passwords = Driver.FindElements(
            By.XPath("//android.widget.EditText[@password='true']"));
        if (passwords.Count >= 1) passwords[0].SendKeys("secret12");
        else Type(ByIdOrHint("entryPassword", "Mínimo 6 caracteres"), "secret12");
        if (passwords.Count >= 2) passwords[1].SendKeys("secret12");
        else Type(ByIdOrHint("entryConfirmarPassword", "Repite tu contraseña"), "secret12");

        // Acepta términos.
        try
        {
            var check = Driver.FindElement(
                By.XPath("//*[@content-desc='chkTerminos' or @resource-id='chkTerminos'] " +
                         "| //android.widget.CheckBox"));
            if (!check.Selected) check.Click();
        }
        catch (NoSuchElementException) { /* layout sin checkbox visible */ }
        catch (WebDriverException) { }

        TapAny(15,
            By.XPath("//*[@content-desc='btnRegistrar' or @resource-id='btnRegistrar' " +
                     "or @text='Crear Cuenta' or @text='Crear cuenta' " +
                     "or @text='Registrarse' or @text='REGISTRARSE']"));

        // El code-behind muestra "✅ Registro Exitoso" en DisplayAlert.
        WaitForAny(35,
            ByContainsText("Exitoso"),
            ByContainsText("éxito"),
            ByContainsText("registrado"),
            ByContainsText("creada"))
            .Displayed.Should().BeTrue();
    }
}
