using CalorieTrackerService.Application.Interface.Application;
using Microsoft.AspNetCore.Mvc;

namespace CalorieTrackerService.Service.WebApi.Controllers
{
    [ApiController]
    [Route("product")]
    public class ProductController : ControllerBase
    {
        private readonly IProductApplication _productApplication;

        public ProductController(IProductApplication productApplication)
        {
            _productApplication = productApplication;
        }

        #region CreateProduct

        #endregion

        #region GetProducts

        #endregion

        #region GetProductsById

        #endregion

        #region GetProductsByName

        #endregion

        #region UpdateProduct

        #endregion

        #region DeleteProduct

        #endregion
    }
}