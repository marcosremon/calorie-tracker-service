namespace CalorieTrackerService.Application.DataTransferObject.Entity
{
    public class AiLogsDTO
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string DetectedText { get; set; } = string.Empty;
        public string DetectedProduct { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}