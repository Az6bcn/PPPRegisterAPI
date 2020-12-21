using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CheckinPPP.Helpers.Middleware
{
    public class RequestLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggerMiddleware> _logger;

        public RequestLoggerMiddleware(RequestDelegate next, ILogger<RequestLoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            // incoming
            var timer = Stopwatch.StartNew();
            var body = await ReadBodyAsJson(context.Request);
            var logModel = GetLogModelAsJsonString(context);

            var sessionId = context.Session.Id;
            _logger.LogWarning("{Request SessionId}", sessionId);
            
            // call next delegate in the pipeline
            await _next(context);
            
            // out going
            var timeElasped = timer.ElapsedMilliseconds;
            timer.Stop();

            logModel.Duration = $"{timeElasped}ms";
            logModel.Body = body;

            var requestTolog = StringToJson(logModel);
            
            _logger.LogWarning("{Request}", requestTolog);
            
            _logger.Log(LogLevel.Information, "Request Response Status: {context.Response.StatusCode }");
            _logger.Log(LogLevel.Information, "End of request with sessionId {SessionId}", sessionId);
            _logger.Log(LogLevel.Information, "{Duration}", timeElasped);
        }

        private LoggingModel GetLogModelAsJsonString(HttpContext context)
        {
            var tokenValid = context.Request.Headers.TryGetValue("Authorization", out var token);


            var accessToken = (tokenValid) ? token.ToString() : "token not passed";
            
            var model = new LoggingModel
            {
                Token = accessToken,
                RequestPath = context.Request.Path,
                Method = context.Request.Path,
                ClientMachineName = Dns.GetHostName(),
                ClientIp = context.Request.Host.Value,
                Host = context.Request.Host.ToString(),
                QueryString = context.Request.QueryString.Value,
            };

            return model;
        }


        private async Task<string> ReadBodyAsJson(HttpRequest request)
        {
            var bodyAstring = string.Empty;
            
            // make sure request can be read multiple times
            request.EnableBuffering();

            using var reader = new StreamReader(request.Body);
            var body = await reader.ReadToEndAsync();

            // reset reading pointer to the beginning
            request.Body.Seek(0, SeekOrigin.Begin);

            //bodyAstring = StringToJson(bodyAstring);

            return bodyAstring;
        }

        private string StringToJson(string bodyAsString)
        {
            var json = JsonConvert.SerializeObject(bodyAsString);

            return json;
        }
        
        private string StringToJson(LoggingModel model)
        {
            var json = JsonConvert.SerializeObject(model);

            return json;
        }
    }
}