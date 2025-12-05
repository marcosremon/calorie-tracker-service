using CalorieTrackerService.Application.Interface.Application;
using CalorieTrackerService.Application.Interface.Repository;

namespace CalorieTrackerService.Application.UseCase
{
    public class UserApplication : IUserApplication
    {
        private readonly IUserRepository _userRepository;

        public UserApplication(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
    }
}