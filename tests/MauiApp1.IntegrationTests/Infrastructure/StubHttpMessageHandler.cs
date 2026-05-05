using System.Net;

namespace MauiApp1.IntegrationTests.Infrastructure;

/// <summary>
/// Handler HTTP stub que permite encolar respuestas y/o configurar una función
/// que construye la respuesta a partir del <c>HttpRequestMessage</c>. Se usa
/// para aislar los tests del backend real.
/// </summary>
public sealed class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage>? _responder;
    private readonly Queue<HttpResponseMessage>? _queue;

    public List<HttpRequestMessage> Requests { get; } = new();

    public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        _responder = responder;
    }

    public StubHttpMessageHandler(IEnumerable<HttpResponseMessage> queue)
    {
        _queue = new Queue<HttpResponseMessage>(queue);
    }

    public static StubHttpMessageHandler ReturnsJson(HttpStatusCode code, string json)
    {
        return new StubHttpMessageHandler(_ =>
        {
            var res = new HttpResponseMessage(code);
            res.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return res;
        });
    }

    public static StubHttpMessageHandler ReturnsStatus(HttpStatusCode code)
    {
        return new StubHttpMessageHandler(_ => new HttpResponseMessage(code));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        Requests.Add(request);
        // Si el body es relevante, bufferízalo para permitir assertions.
        if (request.Content is not null)
            await request.Content.LoadIntoBufferAsync();

        if (_queue is not null && _queue.Count > 0) return _queue.Dequeue();
        if (_responder is not null) return _responder(request);
        return new HttpResponseMessage(HttpStatusCode.NotImplemented);
    }
}
