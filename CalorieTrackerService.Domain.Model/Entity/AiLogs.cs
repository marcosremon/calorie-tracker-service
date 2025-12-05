using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalorieTrackerService.Domain.Model.Entity
{
    [Table("ai_logs")]
    public class AiLogs
    {
        [Key]
        [Column("ai_logs_id")]
        public long AiLogsId { get; set; }

        [Column("image_url")]
        public string ImageUrl { get; set; } = string.Empty;

        [Column("detected_text")]
        public string DetectedText { get; set; } = string.Empty;

        [Column("detected_product")]
        public string DetectedProduct { get; set; } = string.Empty;

        [Column("date")]
        public DateTime Date { get; set; }
    }
}