using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CheckinPPP.Helpers.Middleware
{
    public class RequestLoggerMiddleware
    {
        private readonly ILogger<RequestLoggerMiddleware> _logger;
        private readonly RequestDelegate _next;

        private readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public RequestLoggerMiddleware(RequestDelegate next, ILogger<RequestLoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // incoming
            var timer = Stopwatch.StartNew();
            //var (body, request) = await ReadBodyAsJson(context.Request);
            var bodyAstring = string.Empty;

            // make sure request can be read multiple times
            context.Request.EnableBuffering();

            using var reader = new StreamReader(context.Request.Body);
            bodyAstring = await reader.ReadToEndAsync();

            // reset reading pointer to the beginning
            context.Request.Body.Seek(0, SeekOrigin.Begin);

            //bodyAstring = StringToJson(bodyAstring);

            var logModel = GetLogModelAsJsonString(context);

            var sessionId = Guid.NewGuid();
            _logger.LogWarning("Request SessionId: {Request SessionId}", sessionId);

            // call next delegate in the pipeline
            await _next(context);

            // out going
            var timeElasped = timer.ElapsedMilliseconds;
            timer.Stop();

            logModel.Duration = $"{timeElasped}ms";
            // serialize to json
            logModel.Body = bodyAstring;
            // deserialize to remove unescaped strings /
            //_logger.LogWarning("Body: {body}", StringToJsonAndUnEscapeString(logModel.Body));
            logModel.StatusCode = context.Response.StatusCode;

            var requestTolog = StringToJson(logModel);

            _logger.LogWarning("Request Details: ({sessionId}): {Request}", sessionId, requestTolog);
        }

        private LoggingModel GetLogModelAsJsonString(HttpContext context)
        {
            var tokenValid = context.Request.Headers.TryGetValue("Authorization", out var token);


            var accessToken = tokenValid ? token.ToString() : "token not passed";

            var model = new LoggingModel
            {
                Token = accessToken,
                RequestPath = context.Request.Path,
                Method = context.Request.Path,
                ClientMachineName = Dns.GetHostName(),
                ClientIp = context.Request.Host.Value,
                Host = context.Request.Host.ToString(),
                QueryString = context.Request.QueryString.ToString(),
                User = context.User?.Claims.FirstOrDefault(x => x.Type == "Id")?.Value
            };

            return model;
        }


        private async Task<(string body, HttpRequest request)> ReadBodyAsJson(HttpRequest request)
        {
            var bodyAstring = string.Empty;

            // make sure request can be read multiple times
            request.EnableBuffering();

            using (var reader = new StreamReader(request.Body))
            {
                bodyAstring = await reader.ReadToEndAsync();

                // reset reading pointer to the beginning
                request.Body.Seek(0, SeekOrigin.Begin);
            }

            return (bodyAstring, request);
        }

        private string StringToJson(object bodyAsString)
        {
            var json = JsonConvert.SerializeObject(bodyAsString, Formatting.Indented, jsonSettings);

            return json;
        }

        private object StringToJsonAndUnEscapeString(string bodyAsString)
        {
            var json = JsonConvert.DeserializeObject<object>(bodyAsString, jsonSettings);

            return json;
        }

        private string StringToJson(LoggingModel model)
        {
            var json = JsonConvert.SerializeObject(model, Formatting.Indented, jsonSettings);

            return json;
        }
    }
}