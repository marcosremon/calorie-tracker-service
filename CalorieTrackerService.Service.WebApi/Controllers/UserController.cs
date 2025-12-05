using CalorieTrackerService.Application.Interface.Application;
using CalorieTrackerService.Transversal.JsonInterchange.User.CreateUser;
using Microsoft.AspNetCore.Mvc;

namespace CalorieTrackerService.Service.WebApi.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserApplication _userApplication;

        public UserController(IUserApplication userApplication)
        {
            _userApplication = userApplication;
        }

        #region CreateUser
        [HttpPost("create-user")]
        public async Task<ActionResult<CreateUserResponseJson>> CreateUser([FromBody] CreateUserRequestJson createUserRequestJson)
        {
            CreateUserResponseJson createUserResponseJson = new CreateUserResponseJson();
            try
            {
                
            } 
            catch (Exception ex)
            {
                
            }

            return Ok(createUserResponseJson);
        }
        #endregion
    }
}