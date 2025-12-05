namespace CalorieTrackerService.Application.DataTransferObject.Entity
{
    public class ProductDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public float Calories100g { get; set; }
        public float Carbs100g { get; set; }
        public float Fat100g { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}