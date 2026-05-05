using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Windows;
using System.Text.Json;

namespace MauiApp1.E2ETests.Infrastructure;

public static class AppiumDriverFactory
{
    public static (AppiumDriver Driver, E2EConfig Config) Create()
    {
        var cfg = LoadConfig();
        var opts = new AppiumOptions();
        opts.PlatformName = cfg.Appium.Platform;
	opts.DeviceName = cfg.Appium.DeviceName;
	opts.PlatformVersion = cfg.Appium.PlatformVersion;
	opts.AutomationName = cfg.Appium.Automation;

        if (!string.IsNullOrWhiteSpace(cfg.Appium.AppPath))
            opts.App = cfg.Appium.AppPath;
        else
        {
            opts.AddAdditionalAppiumOption("appPackage", cfg.Appium.AppPackage);
            opts.AddAdditionalAppiumOption("appActivity", cfg.Appium.AppActivity);
        }

        opts.AddAdditionalAppiumOption("noReset", false);
        opts.AddAdditionalAppiumOption("newCommandTimeout", 120);
        opts.AddAdditionalAppiumOption("adbExecTimeout", 60000);

        AppiumDriver driver = cfg.Appium.Platform.ToLowerInvariant() switch
	{
    	"android" => new AndroidDriver(new Uri(cfg.Appium.ServerUrl), opts, TimeSpan.FromMinutes(3)),
    	"ios"     => new IOSDriver(new Uri(cfg.Appium.ServerUrl), opts, TimeSpan.FromMinutes(3)),
    	"windows" => new WindowsDriver(new Uri(cfg.Appium.ServerUrl), opts, TimeSpan.FromMinutes(3)),
    	_ => throw new InvalidOperationException($"Plataforma no soportada: 	{cfg.Appium.Platform}")
	};
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
        return (driver, cfg);
    }

    private static E2EConfig LoadConfig()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (!File.Exists(path))
            throw new FileNotFoundException("Falta appsettings.json en el directorio de salida.", path);
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<E2EConfig>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("appsettings.json vacio");
    }
}

public record E2EConfig(AppiumSection Appium, CredentialsSection Credentials);
public record AppiumSection(
    string ServerUrl, string Platform, string DeviceName, string PlatformVersion,
    string AppPackage, string AppActivity, string AppPath, string Automation);
public record CredentialsSection(
    string AlumnoEmail, string AlumnoPassword,
    string DocenteEmail, string DocentePassword,
    string AdminEmail, string AdminPassword);