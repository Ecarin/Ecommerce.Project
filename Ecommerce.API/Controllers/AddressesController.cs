using Ecommerce.API.Models;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/v1/{shop_name}")]
    public class AddressesController : ControllerBase
    {
        AddressResponse _response;
        AddressesResponse _responses;

        [HttpPost("users/{user_id}/addresses")]
        public async Task<ActionResult<AddressResponse>> AddAddress(string shop_name, string user_id, [FromBody] AddressRequest request)
        {
            try
            {
                if (request == null)
                {
                    _response = new AddressResponse()
                    {
                        Success = false,
                        Message = "Request was null",
                    };
                    return BadRequest(_response);
                }
                var result = await User_AddressService.AddAddressAsync(request, shop_name, user_id);
                if (result == null)
                {
                    _response = new AddressResponse()
                    {
                        Success = false,
                        Message = "Failed to create Address!",
                    };
                    return StatusCode(StatusCodes.Status500InternalServerError, _response);
                }

                _response = new AddressResponse()
                {
                    Success = true,
                    Message = "Address created successfuly",
                    Address = result
                };
                return CreatedAtAction(nameof(AddAddress) ,_response);

            }
            catch (Exception e)
            {
                _response = new AddressResponse()
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpGet("users/{user_id}/addresses")]
        public async Task<ActionResult<AddressesResponse>> GetUserAddressesAsync(string shop_name, string user_id, int? address_id = null)
        {
            try
            {
                var result = await User_AddressService.GetUserAddressesAsync(user_id, shop_name, address_id);
                if (result == null || result.Count() == 0)
                {
                    _responses = new AddressesResponse
                    {
                        Success = false,
                        Message = "No address found for the user!"
                    };
                    return NotFound(_responses);
                }

                _responses = new AddressesResponse
                {
                    Success = true,
                    Message = "Success!",
                    Addresses = result,
                };
                return Ok(_responses);

            }
            catch (Exception e)
            {
                _responses = new AddressesResponse
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _responses);
            }
        }

        [HttpDelete("users/{user_id}/addresses/{address_id}")]
        public async Task<ActionResult<AddressesResponse>> DeleteUserAddressAsync(string shop_name, string user_id, int address_id)
        {
            try
            {
                var result = await User_AddressService.DeleteUserAddressesAsync(user_id, shop_name, address_id);
                if (result == null || result.Count() == 0)
                {
                    _responses = new AddressesResponse
                    {
                        Success = false,
                        Message = "No address found for the user!"
                    };
                    return NotFound(_responses);
                }

                _responses = new AddressesResponse
                {
                    Success = true,
                    Message = "Success!",
                    Addresses = result,
                };
                return Ok(_responses);

            }
            catch (Exception e)
            {
                _responses = new AddressesResponse
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _responses);
            }
        }

        [HttpPut("users/{user_id}/addresses/{address_id}")]
        public async Task<ActionResult<AddressResponse>> UpdateUserAddressAsync(string shop_name, string user_id, int address_id, [FromBody] AddressRequest request)
        {
            try
            {
                if (request == null)
                {
                    _response = new AddressResponse()
                    {
                        Success = false,
                        Message = "Request was null",
                    };
                    return BadRequest(_response);
                }
                var result = await User_AddressService.UpdateUserAddressAsync(request, user_id, address_id, shop_name);
                if (result == null)
                {
                    _response = new AddressResponse()
                    {
                        Success = false,
                        Message = "No address found for the user!",
                    };
                    return StatusCode(StatusCodes.Status500InternalServerError, _response);
                }

                _response = new AddressResponse()
                {
                    Success = true,
                    Message = "Address updated successfuly",
                    Address = result
                };
                return Ok(_response);

            }
            catch (Exception e)
            {
                _response = new AddressResponse
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }
    }
}
