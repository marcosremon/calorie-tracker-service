namespace CalorieTrackerService.Application.DataTransferObject.Entity
{
    public class ConsumptionDto
    {
        public float Grams { get; set; }
        public int State { get; set; }
        public string StateString { get; set; } = string.Empty;
        public float Calories { get; set; }
        public float Protein { get; set; }
        public float Carbs { get; set; }
        public float Fat { get; set; }
        public DateTime Date { get; set; }
    }
}