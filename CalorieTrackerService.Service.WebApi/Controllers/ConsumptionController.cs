using CalorieTrackerService.Application.Interface.Application;
using CalorieTrackerService.Transversal.JsonInterchange.AiLogs.AnalyzeImage;
using CalorieTrackerService.Transversal.JsonInterchange.Consumption.CreateConsumption;
using CalorieTrackerService.Transversal.JsonInterchange.Consumption.DeleteConsumption;
using CalorieTrackerService.Transversal.JsonInterchange.Consumption.GetDayConsumption;
using CalorieTrackerService.Transversal.JsonInterchange.Consumption.GetTodayConsumption;
using CalorieTrackerService.Transversal.JsonInterchange.Consumption.GetWeekConsumption;
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

        #region CreateConsumption
        [HttpPost("create-consumption")]
        public async Task<ActionResult<CreateConsumptionResponseJson>> CreateConsumption([FromBody] CreateConsumptionRequestJson createConsumptionRequestJson)
        {
            CreateConsumptionResponseJson createConsumptionResponseJson = new CreateConsumptionResponseJson();
            try
            {

            }
            catch (Exception ex)
            {

            }

            return Ok(createConsumptionResponseJson);
        }
        #endregion

        #region GetTodayConsumption
        [HttpPost("get-today-consumption")]
        public async Task<ActionResult<GetTodayConsumptionResponseJson>> GetTodayConsumption([FromBody] GetTodayConsumptionRequestJson getTodayConsumptionRequestJson)
        {
            GetTodayConsumptionResponseJson getTodayConsumptionResponseJson = new GetTodayConsumptionResponseJson();
            try
            {

            }
            catch (Exception ex)
            {

            }

            return Ok(getTodayConsumptionResponseJson);
        }
        #endregion

        #region GetDayConsumption
        [HttpPost("get-day-consumption")]
        public async Task<ActionResult<GetDayConsumptionResponseJson>> GetDayConsumption([FromBody] GetDayConsumptionRequestJson getDayConsumptionRequestJson)
        {
            GetDayConsumptionResponseJson getDayConsumptionResponseJson = new GetDayConsumptionResponseJson();
            try
            {

            }
            catch (Exception ex)
            {

            }

            return Ok(getDayConsumptionResponseJson);
        }
        #endregion

        #region GetWeekConsumption
        [HttpPost("get-week-consumption")]
        public async Task<ActionResult<GetWeekConsumptionResponseJson>> GetWeekConsumption([FromBody] GetWeekConsumptionRequestJson getWeekConsumptionRequestJson)
        {
            GetWeekConsumptionResponseJson getWeekConsumptionResponseJson = new GetWeekConsumptionResponseJson();
            try
            {

            }
            catch (Exception ex)
            {

            }

            return Ok(getWeekConsumptionResponseJson);
        }
        #endregion

        #region DeleteConsumption
        [HttpPost("delete-consumption")]
        public async Task<ActionResult<DeleteConsumptionResponseJson>> DeleteConsumption([FromBody] DeleteConsumptionRequestJson deleteConsumptionRequestJson)
        {
            DeleteConsumptionResponseJson deleteConsumptionResponseJson = new DeleteConsumptionResponseJson();
            try
            {

            }
            catch (Exception ex)
            {
            
            }

            return Ok(deleteConsumptionResponseJson);
        }
        #endregion
    }
}