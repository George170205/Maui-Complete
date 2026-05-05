# MauiApp1 — E2E Tests (Appium + NUnit)

Pruebas End-to-End del cliente MAUI contra un emulador/dispositivo real.

## Requisitos

1. **Node.js 18+** + **Appium 2.x**
   ```bash
   npm install -g appium
   appium driver install uiautomator2   # Android
   appium driver install xcuitest       # iOS
   appium driver install windows        # Windows
   ```
2. **Android SDK** con emulador (o dispositivo con depuración USB).
3. **APK compilado** de MauiApp1:
   ```bash
   dotnet publish ../../MauiApp1/MauiApp1 -f net9.0-android -c Release
   ```
4. Rellenar `appsettings.json` → `Credentials` con usuarios de prueba reales.

## Ejecutar

```bash
# Terminal 1 — servidor Appium
appium

# Terminal 2 — tests (los que no encuentren driver se marcan Inconclusive)
dotnet test tests/MauiApp1.E2ETests
```

## Selector policy

Los XPaths usan fallbacks (`@text`, `@resource-id`, `@content-desc`) para
soportar localizaciones. Para estabilizar los tests, añadir
`AutomationId="..."` en cada `Entry`/`Button` de las páginas XAML.
