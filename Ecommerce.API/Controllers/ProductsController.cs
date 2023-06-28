using Ecommerce.Contracts.Models.Requests;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Services;
using Ecommerce.Contracts.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Ecommerce.API.Controllers
{
    [Route("api/v1/{shop_name}/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequest request, string shop_name)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Request body cannot be empty");
                }

                if (!await UserService.IsUserAdmin(request.user_id, shop_name))
                {
                    return Unauthorized();
                }

                if (request.price <= 0)
                {
                    return BadRequest("Price must be greater than zero");
                }

                if (request.quantity < 0)
                {
                    return BadRequest("Quantity must be equal or greater than zero");
                }

                var result = await ProductService.CreateProductAsync(request, shop_name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{product_id}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int product_id, [FromBody] ProductRequest request, string shop_name)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Request body cannot be empty");
                }

                if (!await UserService.IsUserAdmin(request.user_id, shop_name))
                {
                    return Unauthorized();
                }

                if (request.price <= 0)
                {
                    return BadRequest("Price must be greater than zero");
                }

                if (request.quantity < 0)
                {
                    return BadRequest("Quantity must be equal or greater than zero");
                }

                ProductDto updatedProduct = await ProductService.UpdateProductAsync(product_id, request, shop_name);
                return updatedProduct;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteProductAsync(string shop_name, ProductDeleteRequest request)
        {
            try
            {
                if (!await UserService.IsUserAdmin(request.user_id, shop_name))
                {
                    return Unauthorized();
                }
                await ProductService.DeleteProductAsync(shop_name, request.product_id);
                return Ok(new { message = "product removed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{product_id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(string shop_name, int product_id)
        {
            try
            {
                var product = await ProductService.GetProductByIdAsync(shop_name, product_id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet()]
        public async Task<ActionResult<ProductsResponse>> GetProducts(
            string shop_name,
            [FromQuery] List<int>? categoryId,
            [FromQuery] string? name,
            [FromQuery] string sortBy = "product_name",
            [FromQuery] string order = "asc",
            [FromQuery] int limit = 10,
            [FromQuery] int page = 1)
        {
            try
            {
                // Call the GetProducts method to retrieve the list of products
                ProductsResponse productsResponse = await ProductService.GetProductsAsync(shop_name, categoryId, name, sortBy, order, limit, page);
                return Ok(productsResponse);
                // Return the products response as an HTTP 200 (OK) response
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}