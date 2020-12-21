using CheckinPPP.Helpers.Middleware;
using Microsoft.AspNetCore.Builder;

namespace CheckinPPP.Helpers.Extensions
{
    public static class RequestLoggerExtension
    {
        public static IApplicationBuilder UseRequestLoggerExtension(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggerMiddleware>();
        }
    }
}