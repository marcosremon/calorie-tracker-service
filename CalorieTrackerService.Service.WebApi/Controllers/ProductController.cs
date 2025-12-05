using CalorieTrackerService.Application.Interface.Application;
using CalorieTrackerService.Transversal.JsonInterchange.Product.CreateProduct;
using CalorieTrackerService.Transversal.JsonInterchange.Product.DeleteProduct;
using CalorieTrackerService.Transversal.JsonInterchange.Product.GetProductById;
using CalorieTrackerService.Transversal.JsonInterchange.Product.GetProductByName;
using CalorieTrackerService.Transversal.JsonInterchange.Product.GetProducts;
using CalorieTrackerService.Transversal.JsonInterchange.Product.UpdateProduct;
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
        [HttpPost("create-product")]
        public async Task<ActionResult<CreateProductResponseJson>> CreateProduct([FromBody] CreateProductRequestJson createProductRequestJson)
        {
            CreateProductResponseJson createProductResponseJson = new CreateProductResponseJson();
            try
            {

            }
            catch (Exception ex)
            {

            }

            return Ok(createProductResponseJson);
        }
        #endregion

        #region GetProducts
        [HttpPost("get-products")]
        public async Task<ActionResult<GetProductsResponseJson>> GetProducts([FromBody] GetProductsRequestJson getProductsRequestJson)
        {
            GetProductsResponseJson getProductsResponseJson = new GetProductsResponseJson();
            try
            {

            }
            catch (Exception ex)
            {

            }

            return Ok(getProductsResponseJson);
        }
        #endregion

        #region GetProductById
        [HttpPost("get-product-by-id")]
        public async Task<ActionResult<GetProductByIdResponseJson>> GetProductById([FromBody] GetProductByIdRequestJson getProductByIdRequestJson)
        {
            GetProductByIdResponseJson getProductByIdResponseJson = new GetProductByIdResponseJson();
            try
            {

            }
            catch (Exception ex)
            {

            }

            return Ok(getProductByIdResponseJson);
        }
        #endregion

        #region GetProductByName
        [HttpPost("get-product-by-name")]
        public async Task<ActionResult<GetProductByNameResponseJson>> GetProductByName([FromBody] GetProductByNameRequestJson getProductByNameRequestJson)
        {
            GetProductByNameResponseJson getProductByNameResponseJson = new GetProductByNameResponseJson();
            try
            {

            }
            catch (Exception ex)
            {

            }

            return Ok(getProductByNameResponseJson);
        }
        #endregion

        #region UpdateProduct
        [HttpPost("update-product")]
        public async Task<ActionResult<UpdateProductResponseJson>> UpdateProduct([FromBody] UpdateProductRequestJson updateProductRequestJson)
        {
            UpdateProductResponseJson updateProductResponseJson = new UpdateProductResponseJson();
            try
            {

            }
            catch (Exception ex)
            {

            }

            return Ok(updateProductResponseJson);
        }
        #endregion

        #region DeleteProduct
        [HttpPost("delete-product")]
        public async Task<ActionResult<DeleteProductResponseJson>> DeleteProduct([FromBody] DeleteProductRequestJson deleteProductRequestJson)
        {
            DeleteProductResponseJson deleteProductResponseJson = new DeleteProductResponseJson();
            try
            {

            }
            catch (Exception ex)
            {

            }

            return Ok(deleteProductResponseJson);
        }
        #endregion
    }
}