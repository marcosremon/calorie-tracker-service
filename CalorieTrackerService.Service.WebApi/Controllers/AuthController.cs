using CalorieTrackerService.Application.Interface.Application;
using CalorieTrackerService.Transversal.JsonInterchange.Auth.Login;
using Microsoft.AspNetCore.Mvc;

namespace CalorieTrackerService.Service.WebApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthApplication _authApplication;

        public AuthController(IAuthApplication authApplication)
        {
            _authApplication = authApplication;
        }

        #region Login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseJson>> Login([FromBody] LoginRequestJson loginRequestJson)
        {
            LoginResponseJson loginResponseJson = new LoginResponseJson();
            try
            {

            }
            catch (Exception ex)
            {

            }

            return Ok(loginResponseJson);
        }
        #endregion
    }
}