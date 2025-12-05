using CalorieTrackerService.Application.Interface.Application;
using CalorieTrackerService.Application.Interface.Repository;
using CalorieTrackerService.Application.UseCase;
using CalorieTrackerService.Infraestructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace RoutinesGymService.Infraestructure.Persistence.Dependencies
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Application
            services.AddScoped<IUserApplication, UserApplication>();
            services.AddScoped<IAuthApplication, AuthApplication>();
            services.AddScoped<IConsumptionApplication, ConsumptionApplication>();
            services.AddScoped<IProductApplication, ProductApplication>();
            services.AddScoped<IAiLogsApplication, AiLogsApplication>();

            // Repository
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IConsumptionRepository, ConsumptionRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IAiLogsRepository, AiLogsRepository>();

            return services;
        }
    }
}