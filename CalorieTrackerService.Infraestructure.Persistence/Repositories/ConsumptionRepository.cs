using CalorieTrackerService.Application.Interface.Repository;
using RoutinesGymService.Infraestructure.Persistence.Context;

namespace CalorieTrackerService.Infraestructure.Persistence.Repositories
{
    public class ConsumptionRepository : IConsumptionRepository
    {
        private readonly ApplicationDbContext _context;

        public ConsumptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}