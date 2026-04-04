using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public static class SimpleApi
{
    [FunctionName("GetGreeting")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        // 1. Log that the endpoint was hit
        log.LogInformation("GetGreeting function received a request.");

        // 2. Look for a "name" parameter in the URL query string (e.g., ?name=John)
        string name = req.Query["name"];

        // 3. If it wasn't in the URL, check if it was sent in the body of a POST request
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        name = name ?? data?.name;

        // 4. Create the response message based on whether a name was provided
        string responseMessage = string.IsNullOrEmpty(name)
            ? "API executed successfully. Please pass a 'name' in the query string or body."
            : $"Hello, {name}! Your simple C# API is working.";

        // 5. Return an HTTP 200 OK with the message
        return new OkObjectResult(new { status = "Success", message = responseMessage });
    }
}
