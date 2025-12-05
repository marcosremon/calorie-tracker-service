using CalorieTrackerService.Application.Interface.Application;
using CalorieTrackerService.Application.Interface.Repository;

namespace CalorieTrackerService.Application.UseCase
{
    public class ProductApplication : IProductApplication
    {
        private readonly IProductRepository _productRepository;

        public ProductApplication(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
    }
}