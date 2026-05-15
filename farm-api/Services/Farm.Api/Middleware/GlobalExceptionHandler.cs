using System.Net;
using System.Text;
using System.Text.Json;

namespace Farm.Api.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var requestString = GetRequestData(context);
                var errorId = Guid.NewGuid();

                // CRITICAL: actually log the exception. The previous version captured it
                // and threw it away, which made every 500 silent — you saw the status code
                // but had no idea what blew up. Now correlation id ties the response back
                // to the log line.
                _logger.LogError(ex,
                    "Unhandled exception in pipeline. ErrorId={ErrorId} Path={Path} Method={Method}",
                    errorId, context.Request.Path, context.Request.Method);

                var error = new
                {
                    Id = errorId,
                    Status = (short)HttpStatusCode.InternalServerError,
                    Title = "Some kind of error occurred in the API.  Please use the id and contact our support team if the problem persists.",
                    RequestString = _env.IsDevelopment() ? requestString.ToString() : string.Empty,
                    // Surface the exception in dev so the client / browser console shows the
                    // real cause without forcing devs to grep server logs. NEVER in prod.
                    Detail = _env.IsDevelopment() ? ex.ToString() : null,
                };

                await HandleExceptionAsync(context, ex, error);
            }
        }

        private static StringBuilder GetRequestData(HttpContext context)
        {
            var requestBuilder = new StringBuilder();
            requestBuilder.Append("Http Request Information: ");
            requestBuilder.AppendFormat($"Host: {context.Request.Host} ");
            requestBuilder.AppendFormat($"Path: {context.Request.Path} ");

            if (context.Request.QueryString.HasValue)
                requestBuilder.AppendFormat($"QueryString: {context.Request.QueryString.Value} ");

            return requestBuilder;
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, object error)
        {
            var result = JsonSerializer.Serialize(error);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(result);
        }
    }
}
