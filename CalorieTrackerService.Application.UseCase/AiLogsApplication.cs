using CalorieTrackerService.Application.Interface.Application;
using CalorieTrackerService.Application.Interface.Repository;

namespace CalorieTrackerService.Application.UseCase
{
    public class AiLogsApplication : IAiLogsApplication
    {
        private readonly IAiLogsRepository _aiLogsRepository;

        public AiLogsApplication(IAiLogsRepository aiLogsRepository)
        {
            _aiLogsRepository = aiLogsRepository;
        }

    }
}