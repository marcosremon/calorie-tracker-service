using CalorieTrackerService.Application.Interface.Application;
using Microsoft.AspNetCore.Mvc;

namespace CalorieTrackerService.Service.WebApi.Controllers
{
    [ApiController]
    [Route("ia")]
    public class AiLogsController : ControllerBase
    {
        private readonly IAiLogsApplication _aiLogsApplication;

        public AiLogsController(IAiLogsApplication aiLogsApplication)
        {
            _aiLogsApplication = aiLogsApplication;
        }

        #region AnalyzeImage

        #endregion
    }
}