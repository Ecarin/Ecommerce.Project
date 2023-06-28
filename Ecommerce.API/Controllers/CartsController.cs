using Ecommerce.Contracts.Models.Requests;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace YourAppNamespace.Controllers
{
    [ApiController]
    [Route("api/v1/{shop_name}/carts")]
    public class CartsController : ControllerBase
    {
        CartResponse _response;
        [HttpPut]
        public async Task<ActionResult<CartResponse>> UpsertCartAsync([FromBody] CartRequest request, string shop_name)
        {
            try
            {
                var result = await CartService.UpsertCartAsync(request, shop_name);
                _response = new CartResponse()
                {
                    Success = true,
                    Message = "Cart Upserted Successfuly!",
                    Cart = result
                };
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response = new CartResponse()
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpGet("{user_id}")]
        public async Task<ActionResult<CartResponse>> GetCartAsync(string user_id, string shop_name)
        {
            try
            {
                var result = await CartService.GetCartAsync(user_id, shop_name);
                if (result == null)
                {
                    _response = new CartResponse
                    {
                        Success = false,
                        Message = "No cart found for the user!"
                    };
                    return NotFound(_response);
                }

                _response = new CartResponse()
                {
                    Success = true,
                    Message = "Success!",
                    Cart = result
                };
                return Ok(_response);

            }
            catch (Exception e)
            {
                _response = new CartResponse()
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpDelete("{user_id}/{product_Id}")]
        public async Task<ActionResult<CartResponse>> RemoveProductFromCart(string shop_name, string user_id, int product_Id)
        {
            try
            {
                var result = await CartService.RemoveProductFromCartAsync(shop_name, user_id, product_Id);
                if (result == null)
                {
                    _response = new CartResponse()
                    {
                        Success = false,
                        Message = "Product Does not exist in cart!"
                    };
                    return BadRequest(_response);
                }

                _response = new CartResponse()
                {
                    Success = true,
                    Message = "Product Removed Successfuly!",
                    Cart = result,
                };
                return Ok(result);

            }
            catch (Exception e)
            {
                _response = new CartResponse()
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpDelete("{userId}/clear")]
        public async Task<ActionResult<CartResponse>> ClearCart(string shop_name, string userId)
        {
            try
            {
                var result = await CartService.ClearUserCartAsync(shop_name, userId);
                if (result == null)
                {
                    _response = new CartResponse()
                    {
                        Success = false,
                        Message = "User does not have cart yet!"
                    };
                    return BadRequest(_response);
                }

                _response = new CartResponse()
                {
                    Success = true,
                    Message = "Cart Cleared Successfuly!",
                    Cart = result,
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                _response = new CartResponse()
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }
    }
}