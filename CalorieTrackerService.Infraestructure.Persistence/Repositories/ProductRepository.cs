using CalorieTrackerService.Application.Interface.Repository;
using RoutinesGymService.Infraestructure.Persistence.Context;

namespace CalorieTrackerService.Infraestructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}