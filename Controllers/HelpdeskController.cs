using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AIHelpdeskAssistant.Data;
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
        private readonly AppDbContext _context;

        public HelpdeskController(
            GeminiService geminiService,
            AppDbContext context)
        {
            _geminiService = geminiService;
            _context = context;
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

                // Save the request and AI response to the database
                SupportRequest supportRequest = new SupportRequest
                {
                    Problem = request.Problem,
                    Category = response.Category ?? "",
                    Priority = response.Priority ?? "",
                    SuggestedSolution = response.SuggestedSolution ?? "",
                    CreatedAt = DateTime.Now
                };

                _context.SupportRequests.Add(supportRequest);
                await _context.SaveChangesAsync();

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

        [HttpGet("requests")]
        public async Task<IActionResult> GetRequests()
        {
            var requests = await _context.SupportRequests
                .OrderByDescending(request => request.CreatedAt)
                .ToListAsync();

            return Ok(requests);
        }
    }
}