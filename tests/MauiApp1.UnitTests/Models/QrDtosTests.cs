namespace MauiApp1.UnitTests.Models;

/// <summary>
/// Pruebas de los DTOs del flujo QR (PDF §3.3).
/// </summary>
public class QrDtosTests
{
    [Fact(DisplayName = "GenerarQrRequest: record mantiene sus valores y es comparable por igualdad")]
    public void GenerarQrRequest_RecordEquality()
    {
        var a = new GenerarQrRequest(10, 5);
        var b = new GenerarQrRequest(10, 5);
        a.Should().Be(b);
        a.SesionClaseID.Should().Be(10);
        a.DuracionMinutos.Should().Be(5);
    }

    [Fact(DisplayName = "GenerarQrRequest: duración puede ser null")]
    public void GenerarQrRequest_NullDuration()
    {
        var r = new GenerarQrRequest(1, null);
        r.DuracionMinutos.Should().BeNull();
    }

    [Fact(DisplayName = "GenerarQrResponse: expira en el futuro")]
    public void GenerarQrResponse_ExpiraEn_Future()
    {
        var r = new GenerarQrResponse
        {
            QrGeneradoID = 1,
            Token = "abc",
            ExpiraEn = DateTime.UtcNow.AddMinutes(10),
            DuracionMinutos = 10,
            ImagenBase64 = "data:image/png;base64,iVBORw0K..."
        };

        r.ExpiraEn.Should().BeAfter(DateTime.UtcNow);
        r.ImagenBase64.Should().StartWith("data:image/png;base64,");
    }

    [Fact(DisplayName = "ValidarQrRequest: record con lat/long opcionales")]
    public void ValidarQrRequest_OptionalCoords()
    {
        var r = new ValidarQrRequest("tok", 7);
        r.Latitud.Should().BeNull();
        r.Longitud.Should().BeNull();

        var r2 = new ValidarQrRequest("tok", 7, 19.4326m, -99.1332m);
        r2.Latitud.Should().Be(19.4326m);
        r2.Longitud.Should().Be(-99.1332m);
    }

    [Fact(DisplayName = "ValidarQrResponse: valores por defecto seguros")]
    public void ValidarQrResponse_Defaults()
    {
        var r = new ValidarQrResponse();
        r.Message.Should().BeEmpty();
        r.AsistenciaID.Should().BeNull();
    }
}
