using Serilog;
using System.Diagnostics;
using System.Text;

namespace BillingSystem.API.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, Serilog.ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            context.Request.EnableBuffering();

            var requestBody = await ReadBody(context.Request.Body);
            context.Request.Body.Position = 0;

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            responseBody.Seek(0, SeekOrigin.Begin);
            var response = await new StreamReader(responseBody).ReadToEndAsync();

            sw.Stop();

            var correlationId = context.Items["X-Correlation-Id"]?.ToString();

            _logger.Information(
                "HTTP {Method} {Path} {StatusCode} in {Elapsed}ms\nCorrelationId: {CorrelationId}\nRequest: {Request}\nResponse: {Response}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                sw.ElapsedMilliseconds,
                correlationId,
                Sanitize(requestBody),
                Sanitize(response));

            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }

        private async Task<string> ReadBody(Stream body)
        {
            using var reader = new StreamReader(body, Encoding.UTF8, leaveOpen: true);
            return await reader.ReadToEndAsync();
        }

        private string Sanitize(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            return input
                .Replace("password", "******")
                .Replace("token", "******")
                .Replace("secret", "******");
        }


    }
}
