using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalorieTrackerService.Domain.Model.Entity
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("product_id")]
        public long ProductId { get; set; }

        [Column("product_name")]
        public string ProductName { get; set; } = string.Empty;

        [Column("brand")]
        public string Brand { get; set; } = string.Empty;

        [Column("calories_100g")]
        public float Calories100g { get; set; }

        [Column("carbs_100g")]
        public float Carbs100g { get; set; }

        [Column("fat_100g")]
        public float Fat100g { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}