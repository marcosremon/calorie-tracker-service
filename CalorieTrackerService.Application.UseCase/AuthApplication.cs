using CalorieTrackerService.Application.Interface.Application;
using CalorieTrackerService.Application.Interface.Repository;

namespace CalorieTrackerService.Application.UseCase
{
    public class AuthApplication : IAuthApplication
    {
        private readonly IAuthRepository _authRepository;

        public AuthApplication(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }
    }
}