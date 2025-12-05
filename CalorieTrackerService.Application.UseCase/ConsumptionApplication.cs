using CalorieTrackerService.Application.Interface.Application;
using CalorieTrackerService.Application.Interface.Repository;

namespace CalorieTrackerService.Application.UseCase
{
    public class ConsumptionApplication : IConsumptionApplication
    {
        private readonly IConsumptionRepository _consumptionRepository;

        public ConsumptionApplication(IConsumptionRepository consumptionRepository)
        {
            _consumptionRepository = consumptionRepository;
        }
    }
}