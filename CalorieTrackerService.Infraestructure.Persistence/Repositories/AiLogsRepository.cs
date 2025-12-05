using CalorieTrackerService.Application.Interface.Repository;
using RoutinesGymService.Infraestructure.Persistence.Context;

namespace CalorieTrackerService.Infraestructure.Persistence.Repositories
{
    public class AiLogsRepository : IAiLogsRepository
    {
        private readonly ApplicationDbContext _context;

        public AiLogsRepository(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}