using CalorieTrackerService.Application.Interface.Repository;
using RoutinesGymService.Infraestructure.Persistence.Context;

namespace CalorieTrackerService.Infraestructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}