using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalorieTrackerService.Domain.Model.Entity
{
    [Table("consumption")]
    public class Consumption
    {
        [Key]
        [Column("consumption_id")]
        public long ConsumptionId { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Column("product_id")]
        public long ProductId { get; set; }

        [Column("grams")]
        public float Grams { get; set; }

        [Column("state")]
        public int State { get; set; }

        [Column("state_string")]
        public string StateString { get; set; } = string.Empty;

        [Column("calories")]
        public float Calories { get; set; }

        [Column("protein")]
        public float Protein { get; set; }

        [Column("carbs")]
        public float Carbs { get; set; }

        [Column("fat")]
        public float Fat { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }
    }
}