using Microsoft.Extensions.DependencyInjection;

namespace RoutinesGymService.Infraestructure.Persistence.Dependencies
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Application
            // ejemplo
            //services.AddScoped<IUserApplication, UserApplication>();

            // Repository
            // ejemplo
            //services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}