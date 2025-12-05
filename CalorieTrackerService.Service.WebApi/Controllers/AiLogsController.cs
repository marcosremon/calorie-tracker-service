using CalorieTrackerService.Application.Interface.Application;
using CalorieTrackerService.Transversal.JsonInterchange.AiLogs.AnalyzeImage;
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
        [HttpPost("analyze-image")]
        public async Task<ActionResult<AnalyzeImageResponseJson>> AnalyzeImage([FromBody] AnalyzeImageRequestJson analyzeImageRequestJson)
        {
            AnalyzeImageResponseJson analyzeImageResponseJson = new AnalyzeImageResponseJson();
            try
            {

            } 
            catch (Exception ex)
            {

            }

            return Ok(analyzeImageResponseJson);
        }
        #endregion
    }
}