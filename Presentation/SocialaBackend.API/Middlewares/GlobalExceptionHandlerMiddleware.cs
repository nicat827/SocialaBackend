using SocialaBackend.Application.Exceptions.Base;

namespace SocialaBackend.API.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (BaseException ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex.Code;
                var obj = new { statusCode = ex.Code, message = ex.Message };
                await context.Response.WriteAsJsonAsync(obj);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;
                var obj = new { statusCode = 500, message = ex.Message };
                await context.Response.WriteAsJsonAsync(obj);
            }
               
        }
    }
}
