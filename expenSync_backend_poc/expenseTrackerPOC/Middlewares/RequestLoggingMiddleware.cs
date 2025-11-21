namespace expenseTrackerPOC.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            logger.LogInformation($"Incoming Request : {context.Request.Method} {context.Request.Path}");
            await _next(context);
            logger.LogInformation($"Response Status : {context.Response.StatusCode}");
        }        
    }
    public static class RequestMiddlewareExtensions
    {
        public static void UseRequestLogging(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
