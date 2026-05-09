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

                var error = new
                {
                    Id = Guid.NewGuid(),
                    Status = (short)HttpStatusCode.InternalServerError,
                    Title = "Some kind of error occurred in the API.  Please use the id and contact our support team if the problem persists.",
                    RequestString = _env.IsDevelopment() ? requestString.ToString() : string.Empty,
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
