using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreStudy.Web.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            //_logger.LogDebug();
            //_logger.LogError();
            _logger.LogInformation(await FormatRequest(context.Request));

            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                _logger.LogInformation(await FormatResponse(context.Response));
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }


        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var requestBody = Encoding.UTF8.GetString(buffer);

            request.Body.Seek(0, SeekOrigin.Begin);

            var requestLog = new StringBuilder();

            requestLog.AppendLine("[Request Header]");
            foreach (var header in request.Headers)
            {
                foreach (var headerValue in header.Value)
                {
                    requestLog.AppendLine($"{header.Key}: {headerValue}");
                }
            }

            requestLog.AppendLine();
            requestLog.AppendLine("[Request Body]");
            requestLog.AppendLine(requestBody);

            return requestLog.ToString();
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            var responseLog = new StringBuilder();

            responseLog.AppendLine("[Response Header]");
            foreach (var header in response.Headers)
            {
                foreach (var headerValue in header.Value)
                {
                    responseLog.AppendLine($"{header.Key}: {headerValue}");
                }
            }

            responseLog.AppendLine();
            responseLog.AppendLine("[Response Body]");
            responseLog.AppendLine(responseBody);

            return responseLog.ToString();
        }


    }

    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
