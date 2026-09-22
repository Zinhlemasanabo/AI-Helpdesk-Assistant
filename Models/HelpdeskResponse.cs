namespace AIHelpdeskAssistant.Models
{
    public class HelpdeskResponse
    {
        public string Category { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public string SuggestedSolution { get; set; } = string.Empty;
    }
}