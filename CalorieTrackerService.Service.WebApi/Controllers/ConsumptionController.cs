using CalorieTrackerService.Application.Interface.Application;
using Microsoft.AspNetCore.Mvc;

namespace CalorieTrackerService.Service.WebApi.Controllers
{
    [ApiController]
    [Route("consumption")]
    public class ConsumptionController : ControllerBase
    {
        private readonly IConsumptionApplication _consumptionApplication;

        public ConsumptionController(IConsumptionApplication consumptionApplication)
        {
            _consumptionApplication = consumptionApplication;
        }
    }
}