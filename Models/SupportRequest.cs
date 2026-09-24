namespace AIHelpdeskAssistant.Models
{
    public class SupportRequest
    {
        public int Id { get; set; }

        public string Problem { get; set; } = "";

        public string Category { get; set; } = "";

        public string Priority { get; set; } = "";

        public string SuggestedSolution { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}