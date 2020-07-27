using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CoreStudy.Web.Models.Posts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreStudy.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly HttpClient _httpClient;

        public TestController(
            ILogger<TestController> logger,
            HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        [HttpPost]
        [Route("{*route}")]
        public async Task<IActionResult> Relay([FromRoute] string route)
        {
            try
            {
                var body = String.Empty;
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    body = await reader.ReadToEndAsync();
                }

                var title = JsonSerializer.Deserialize<PostModel>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }).Title;

                _logger.LogInformation("转发" + title);

                var httpContent = new StringContent(body);

                httpContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

                if (Request.QueryString.HasValue)
                {
                    route = route + Request.QueryString.Value;
                }

                string url = "" + route;
                var response = await _httpClient.PostAsync(url, httpContent);
                var str = await response.Content.ReadAsStringAsync();

                return new ContentResult
                {
                    Content = str,
                    ContentType = response.Content.Headers.ContentType.ToString(),
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Ok();
            }
        }

    }
}
