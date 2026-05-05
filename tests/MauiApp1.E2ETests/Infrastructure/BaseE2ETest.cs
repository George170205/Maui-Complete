using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Support.UI;

namespace MauiApp1.E2ETests.Infrastructure;

/// <summary>
/// Base para los tests E2E. Crea el driver en <c>[SetUp]</c> (si Appium está
/// disponible) y lo destruye en <c>[TearDown]</c>. Si el servidor Appium no
/// está corriendo los tests se marcan como <c>Inconclusive</c> para no romper
/// la suite en CI sin dispositivo.
///
/// Helpers expuestos:
///   <see cref="ById"/>            — por AutomationId (content-desc o resource-id).
///   <see cref="ByHint"/>           — por placeholder/hint (atributo <c>hint</c>).
///   <see cref="ByIdOrHint"/>       — combinación: AutomationId + hint + text.
///   <see cref="ByText"/>           — texto exacto.
///   <see cref="ByContainsText"/>   — texto parcial.
///   <see cref="WaitForAny"/>       — espera el primer selector que aparezca.
///   <see cref="TapAny"/>           — tap sobre el primer selector visible.
///
/// Nota: en MAUI .NET 9 sobre Android, <c>AutomationId</c> NO siempre se
/// propaga como <c>content-desc</c> en el árbol UiAutomator2 (depende del
/// renderer del control). Para Entry/Picker es más confiable matchear por
/// el atributo <c>hint</c> o por el <c>text</c> visible.
/// </summary>
[Category("E2E")]
public abstract class BaseE2ETest
{
    protected AppiumDriver Driver { get; private set; } = null!;
    protected E2EConfig Config { get; private set; } = null!;

    [SetUp]
    public void SetUpDriver()
    {
        try
        {
            (Driver, Config) = AppiumDriverFactory.Create();
        }
        catch (Exception ex) when (ex is WebDriverException or HttpRequestException or IOException)
        {
            Assert.Inconclusive($"Appium/driver no disponible: {ex.GetType().Name} — {ex.Message}");
        }
    }

    [TearDown]
    public void TearDownDriver()
    {
        try { Driver?.Quit(); } catch { /* best-effort */ }
    }

    // ---- Selectors ----

    protected static By ById(string automationId) =>
        By.XPath($"//*[@content-desc='{automationId}' or @resource-id='{automationId}']");

    protected static By ByHint(string hint) =>
        By.XPath($"//android.widget.EditText[@hint='{hint}' or @text='{hint}']");

    /// <summary>
    /// Match combinado: AutomationId (content-desc/resource-id) + hint/text
    /// visible. La forma preferida porque cubre el caso en que MAUI no
    /// propaga el AutomationId al árbol Android.
    /// </summary>
    protected static By ByIdOrHint(string automationId, params string[] hintsOrTexts)
    {
        var orParts = new List<string>
        {
            $"@content-desc='{automationId}'",
            $"@resource-id='{automationId}'"
        };
        foreach (var h in hintsOrTexts)
        {
            var escaped = h.Replace("'", "&apos;");
            orParts.Add($"@hint='{escaped}'");
            orParts.Add($"@text='{escaped}'");
        }
        return By.XPath($"//*[{string.Join(" or ", orParts)}]");
    }

    /// <summary>Selector por texto exacto (atributo <c>text</c>).</summary>
    protected static By ByText(string text) =>
        By.XPath($"//*[@text='{text}']");

    /// <summary>Selector "contiene texto" (case-sensitive).</summary>
    protected static By ByContainsText(string text) =>
        By.XPath($"//*[contains(@text,'{text}')]");

    /// <summary>Primer EditText (Entry/Picker) por posición — fallback.</summary>
    protected static By NthEditText(int oneBasedIndex) =>
        By.XPath($"(//android.widget.EditText)[{oneBasedIndex}]");

    // ---- Helpers ----
    protected IWebElement Wait(By by, int timeoutSeconds = 45)
    {
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
        return wait.Until(d =>
        {
            try { var e = d.FindElement(by); return e != null && e.Displayed ? e : null; }
            catch (NoSuchElementException) { return null; }
            catch (StaleElementReferenceException) { return null; }
        })!;
    }

    /// <summary>Espera hasta que aparezca cualquiera de los selectores.</summary>
    protected IWebElement WaitForAny(int timeoutSeconds, params By[] selectors)
    {
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
        return wait.Until(d =>
        {
            foreach (var by in selectors)
            {
                try
                {
                    var e = d.FindElement(by);
                    if (e != null && e.Displayed) return e;
                }
                catch (NoSuchElementException) { }
                catch (StaleElementReferenceException) { }
            }
            return null;
        })!;
    }

    protected void Type(By by, string text)
    {
        var el = Wait(by);
        try { el.Clear(); } catch { /* algunos Entry MAUI no soportan Clear */ }
        el.SendKeys(text);
    }

    protected void Tap(By by) => Wait(by).Click();

    protected void TapAny(int timeoutSeconds, params By[] selectors)
        => WaitForAny(timeoutSeconds, selectors).Click();

    /// <summary>
    /// Útil para depurar selectores cuando un test falla. Imprime el árbol
    /// XML de la pantalla actual al output de NUnit.
    /// </summary>
    protected void DumpPageSource()
    {
        try
        {
            TestContext.WriteLine("=== PAGE SOURCE ===");
            TestContext.WriteLine(Driver.PageSource);
            TestContext.WriteLine("=== END PAGE SOURCE ===");
        }
        catch (Exception ex)
        {
            TestContext.WriteLine($"Error obteniendo page source: {ex.Message}");
        }
    }
}
