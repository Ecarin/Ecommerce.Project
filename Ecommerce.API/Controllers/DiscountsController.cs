using Ecommerce.Contracts.Models.Requests;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/v1/{shop_name}/discounts")]
    public class DiscountsController : ControllerBase
    {
        DiscountResponse _response;
        DiscountsResponse _responses;

        [HttpGet]
        public async Task<ActionResult<DiscountsResponse>> GetAllDiscounts(string shop_name)
        {
            try
            {
                var result = await DiscountService.GetDiscountsAsync(shop_name);
                if (result == null || result.Count() == 0)
                {
                    _responses = new DiscountsResponse
                    {
                        Success = false,
                        Message = "no discounts found!"
                    };
                    return NotFound(_responses);
                }

                _responses = new DiscountsResponse
                {
                    Success = true,
                    Message = "Success!",
                    Discounts = result,
                };
                return Ok(_responses);

            }
            catch (Exception e)
            {
                _responses = new DiscountsResponse()
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _responses);
            }
        }

        [HttpPost]
        public async Task<ActionResult<DiscountResponse>> CreateDiscount(string shop_name, [FromBody] DiscountRequest request)
        {
            try
            {
                if (request == null)
                {
                    _response = new DiscountResponse()
                    {
                        Success = false,
                        Message = "request can not be null"
                    };
                    return BadRequest(_response);
                }

                if (await UserService.IsUserAdmin(request.user_id, shop_name) == false)
                {
                    _response = new DiscountResponse()
                    {
                        Success = false,
                        Message = "only admin can create discounts!"
                    };
                    return Unauthorized(_response);
                }

                if (request.discount_type != "price" && request.discount_type != "percent")
                {
                    _response = new DiscountResponse()
                    {
                        Success = false,
                        Message = "discount type not supported!"
                    };
                    return BadRequest(_response);
                }

                if ((request.category_id == null && request.product_id == null) ||
                    (request.category_id != null && request.product_id != null))
                {
                    _response = new DiscountResponse()
                    {
                        Success = false,
                        Message = "params conflict!"
                    };
                    return BadRequest(_response);
                }

                var result = await DiscountService.CreateDiscountAsync(request, shop_name);
                if (result == null)
                {
                    _response = new DiscountResponse()
                    {
                        Success = false,
                        Message = "failed to create discount!"
                    };
                    return StatusCode(StatusCodes.Status500InternalServerError, _response);
                }

                _response = new DiscountResponse()
                {
                    Success = true,
                    Message = "Success!",
                    Discount = result
                };
                return CreatedAtAction(nameof(CreateDiscount), _response);

            }
            catch (Exception e)
            {
                _responses = new DiscountsResponse()
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _responses);
            }
        }

        [HttpPut("{discountId}")]
        public async Task<ActionResult<DiscountResponse>> UpdateDiscount(string shop_name, int discountId, [FromBody] DiscountRequest request)
        {
            try
            {
                if (request == null)
                {
                    _response = new DiscountResponse()
                    {
                        Success = false,
                        Message = "request can not be null"
                    };
                    return BadRequest(_response);
                }

                if (await UserService.IsUserAdmin(request.user_id, shop_name) == false)
                {
                    _response = new DiscountResponse()
                    {
                        Success = false,
                        Message = "only admin can update discounts!"
                    };
                    return Unauthorized(_response);
                }

                if (request.discount_type != "price" && request.discount_type != "percent")
                {
                    _response = new DiscountResponse()
                    {
                        Success = false,
                        Message = "discount type not supported!"
                    };
                    return BadRequest(_response);
                }

                if ((request.category_id == null && request.product_id == null) ||
                    (request.category_id != null && request.product_id != null))
                {
                    _response = new DiscountResponse()
                    {
                        Success = false,
                        Message = "params conflict!"
                    };
                    return BadRequest(_response);
                }

                var result = await DiscountService.UpdateDiscountAsync(discountId, request, shop_name);
                if (result == null)
                {
                    _response = new DiscountResponse()
                    {
                        Success = false,
                        Message = "failed to update discount!"
                    };
                    return StatusCode(StatusCodes.Status500InternalServerError, _response);
                }

                _response = new DiscountResponse()
                {
                    Success = true,
                    Message = "Success!",
                    Discount = result
                };
                return Ok(_response);

            }
            catch (Exception e)
            {
                _responses = new DiscountsResponse()
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _responses);
            }
        }


        [HttpDelete("{discountId}")]
        public async Task<ActionResult<DiscountsResponse>> DeleteDiscount(string shop_name, int discountId, string user_id)
        {
            try
            {
                if (await UserService.IsUserAdmin(user_id, shop_name) == false)
                {
                    _responses = new DiscountsResponse()
                    {
                        Success = false,
                        Message = "only admin can delete discounts!"
                    };
                    return Unauthorized(_responses);
                }

                var result = await DiscountService.DeleteDiscountAsync(discountId, shop_name);
                if (result == null || result.Count() == 0)
                {
                    _responses = new DiscountsResponse
                    {
                        Success = false,
                        Message = "no discounts found!"
                    };
                    return NotFound(_responses);
                }

                _responses = new DiscountsResponse()
                {
                    Success = true,
                    Message = "Success!",
                    Discounts = result
                };
                return Ok(_responses);

            }
            catch (Exception e)
            {
                _responses = new DiscountsResponse()
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _responses);
            }
        }
    }
}