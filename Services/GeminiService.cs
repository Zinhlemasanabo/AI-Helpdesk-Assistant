using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AIHelpdeskAssistant.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GeminiApiKey"] ?? "";
        }

        public async Task<string> AskGemini(string problem)
        {
            if (string.IsNullOrWhiteSpace(problem))
            {
                throw new ArgumentException(
                    "Please enter an IT problem."
                );
            }

            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new InvalidOperationException(
                    "Gemini API key is missing."
                );
            }

            string url =
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.6-flash:generateContent?key={_apiKey}";

            string prompt =
     "You are an IT Helpdesk Assistant.\n\n" +
     "Analyse the following IT problem:\n" +
     problem + "\n\n" +
     "Return ONLY valid JSON.\n" +
     "Do not include markdown or any introduction.\n\n" +
     "Use exactly this structure:\n" +
     "{\n" +
     "  \"category\": \"Network\",\n" +
     "  \"priority\": \"Medium\",\n" +
     "  \"suggestedSolution\": \"Clear troubleshooting instructions\"\n" +
     "}\n\n" +
     "Category must be one of: Hardware, Software, Network, Security, Account, Other.\n" +
     "Priority must be one of: Low, Medium, High.\n" +
     "Keep the suggested solution concise and beginner-friendly.";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = prompt
                            }
                        }
                    }
                },
                generationConfig = new
                {
                    responseMimeType = "application/json"
                }
            };

            var response =
                await _httpClient.PostAsJsonAsync(url, requestBody);

            string json =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Gemini API Error: {response.StatusCode}\n{json}"
                );
            }

            using JsonDocument document =
                JsonDocument.Parse(json);

            string answer = document
                .RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "";

            return answer;
        }
    }
}