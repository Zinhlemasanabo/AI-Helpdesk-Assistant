using Microsoft.AspNetCore.Mvc;
using AIHelpdeskAssistant.Models;
using AIHelpdeskAssistant.Services;
using System.Text.Json;

namespace AIHelpdeskAssistant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelpdeskController : ControllerBase
    {
        private readonly GeminiService _geminiService;

        public HelpdeskController(GeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost("analyse")]
        public async Task<IActionResult> AnalyseProblem(HelpdeskRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Problem))
            {
                return BadRequest(new
                {
                    message = "Please enter an IT problem."
                });
            }

            try
            {
                string aiAnswer =
                    await _geminiService.AskGemini(request.Problem);

                HelpdeskResponse? response =
                    JsonSerializer.Deserialize<HelpdeskResponse>(
                        aiAnswer,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (response == null)
                {
                    return StatusCode(500, new
                    {
                        message = "The AI returned an invalid response."
                    });
                }

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "The AI service is temporarily unavailable. Please try again."
                });
            }
        }
    }
}