using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using BiblioServer.Models;

namespace BiblioServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AIController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public AIController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }


        [HttpPost]
        public async Task<IActionResult> GenerateCompletion([FromBody] AIModel model)
        {
            if(model.Body.Length < 4 && model.Body.Length > 30)
            {
                return BadRequest("Too short or long");
            }

            try
            {
                var requestData = new
                {
                    messages = new[]
                    {
                        new { role = "system", content = "Biblio is a platform for free reading of electronic books and more." },
                        new { role = "user", content = model.Body }
                    },
                    temperature = 0.7,
                    max_tokens = 64,
                    top_p = 1,
                    model = "ft:gpt-3.5-turbo-0125:personal::95KTzPPV"
                };

                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer ---");

                var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestData);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    using (JsonDocument document = JsonDocument.Parse(responseBody))
                    {
                        JsonElement root = document.RootElement;
                        var choices = root.GetProperty("choices");
                        var firstChoice = choices[0];
                        var completionMessage = firstChoice.GetProperty("message").GetProperty("content").GetString();

                        var withoutUrl = RemoveUrlFromCompletion(completionMessage);
                        return Ok(withoutUrl);
                    }
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Failed to generate completion");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        private string RemoveUrlFromCompletion(string completionMessage)
        {
            int startIndex = completionMessage.IndexOf('[');
            int endIndex = completionMessage.IndexOf(']');

            if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
            {
                return completionMessage.Remove(startIndex, endIndex - startIndex + 1).Trim();
            }
            return completionMessage;
        }
    }

}

