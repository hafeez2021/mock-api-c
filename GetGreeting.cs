using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace SimpleMockApi
{
    public class GetGreeting
    {
        private readonly ILogger _logger;

        public GetGreeting(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetGreeting>();
        }

        [Function("GetGreeting")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("GetGreeting function received a request.");

            // 1. Look for a "name" parameter in the URL query string
            string? name = req.Query["name"];

            // 2. If it wasn't in the URL, check if it was sent in the body of a POST request
            if (string.IsNullOrEmpty(name))
            {
                using var reader = new StreamReader(req.Body);
                var requestBody = await reader.ReadToEndAsync();
                
                if (!string.IsNullOrEmpty(requestBody))
                {
                    // Parse the JSON body securely
                    using var document = JsonDocument.Parse(requestBody);
                    if (document.RootElement.TryGetProperty("name", out JsonElement nameElement))
                    {
                        name = nameElement.GetString();
                    }
                }
            }

            // 3. Create the response message
            string responseMessage = string.IsNullOrEmpty(name)
                ? "API executed successfully. Please pass a 'name' in the query string or JSON body."
                : $"Hello, {name}! Your simple C# API is working.";

            // 4. Build and return the HTTP 200 OK response
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new { status = "Success", message = responseMessage });

            return response;
        }
    }
}