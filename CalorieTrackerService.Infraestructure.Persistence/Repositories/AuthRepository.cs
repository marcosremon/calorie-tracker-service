using CalorieTrackerService.Application.Interface.Repository;
using RoutinesGymService.Infraestructure.Persistence.Context;

namespace CalorieTrackerService.Infraestructure.Persistence.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}