using System.Text;

namespace NETMockServer.Middleware;

public class RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Log Request
        context.Request.EnableBuffering();

        var requestBody = await ReadStreamAsync(context.Request.Body);
        context.Request.Body.Position = 0;

        logger.LogInformation("HTTP {Method} {Path} RequestBody: {Body}", context.Request.Method, context.Request.Path, requestBody);

        // Capture response
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await next(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        logger.LogInformation("HTTP {Method} {Path} Response {StatusCode}: {ResponseBody}", context.Request.Method, context.Request.Path, context.Response.StatusCode, responseText);

        await responseBody.CopyToAsync(originalBodyStream);
    }

    private static async Task<string> ReadStreamAsync(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        var text = await reader.ReadToEndAsync();
        stream.Seek(0, SeekOrigin.Begin);

        return string.IsNullOrWhiteSpace(text) ? "<empty>" : text;
    }
}