using Ecommerce.Contracts.Models.Requests;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [Route("api/v1/{shop_name}")]
    [ApiController]
    public class BotsDataController : ControllerBase
    {
        BotDataResponse _response;
        [HttpGet]
        public async Task<ActionResult<BotDataResponse>> GetBotDataAsync(string shop_name)
        {
            try
            {
                var result = await BotDataService.GetBotData(shop_name);
                if (result == null)
                {
                    _response = new BotDataResponse()
                    {
                        Success = false,
                        Message = "No Bot found for this shop_name!",
                    };
                    return StatusCode(StatusCodes.Status500InternalServerError, _response);
                }

                _response = new BotDataResponse()
                {
                    Success = true,
                    BotData = result
                };
                return Ok(_response);

            }
            catch (Exception e)
            {
                _response = new BotDataResponse
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPut]
        public async Task<ActionResult<BotDataResponse>> UpdateBotDataAsync(string shop_name, [FromBody] BotDataRequest request)
        {
            try
            {
                if (request == null)
                {
                    _response = new BotDataResponse()
                    {
                        Success = false,
                        Message = "Request was null",
                    };
                    return BadRequest(_response);
                }

                if (await UserService.IsUserAdmin(request.user_id, shop_name) == false)
                {
                    _response = new BotDataResponse()
                    {
                        Success = false,
                        Message = "only admin can update bot data!"
                    };
                    return Unauthorized(_response);
                }

                var result = await BotDataService.UpdateBotData(shop_name, request);
                if (result == null)
                {
                    _response = new BotDataResponse()
                    {
                        Success = false,
                        Message = "No Bot found for this shop_name!",
                    };
                    return StatusCode(StatusCodes.Status500InternalServerError, _response);
                }

                _response = new BotDataResponse()
                {
                    Success = true,
                    BotData = result
                };
                return Ok(_response);

            }
            catch (Exception e)
            {
                _response = new BotDataResponse
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }
    }
}
