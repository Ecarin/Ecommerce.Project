using Ecommerce.Contracts.Models.Requests;
using Ecommerce.Contracts.Models.Requests.Pti7;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Models.Tables.Pti7;
using Ecommerce.Contracts.Services;
using Ecommerce.Contracts.Services.Pti7;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Requests.Abstractions;

namespace Ecommerce.API.Controllers.Pti7
{
    [Route("api/v1/pti7/master")]
    [ApiController]
    public class MasterController : ControllerBase
    {
        MasterResponse _response;
        MastersResponse _responses;

        [HttpPost("photo")]
        public async Task<ActionResult> PostPhoto([FromBody] PhotoRequest request)
        {
            if (request == null)
            {
                return BadRequest("No photo provided.");
            }

            if (request.photo_base64.Length == 0)
            {
                return BadRequest("Photo is empty.");
            }

            // Decode the base64-encoded photo string.
            var bytes = Convert.FromBase64String(request.photo_base64);

            // Generate a unique filename for the photo.
            var filename = $"{Guid.NewGuid()}.png";
            string path = $"pti7//masters";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Save the photo to disk.
            await System.IO.File.WriteAllBytesAsync($"{path}//{filename}", bytes);

            // Return a response containing path.
            return Ok($"{path}//{filename}");
        }

        [HttpGet]
        public async Task<ActionResult<MastersResponse>> GetMasters()
        {
            try
            {
                var result = await MasterService.GetMasters();
                if (result == null || result.Count() == 0)
                {
                    _responses = new MastersResponse
                    {
                        Success = false,
                        Message = "No Master found for pti7!"
                    };
                    return NotFound(_responses);
                }

                _responses = new MastersResponse
                {
                    Success = true,
                    Masters = result
                };
                return Ok(_responses);
            }
            catch (Exception e)
            {
                _responses = new MastersResponse
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _responses);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MasterResponse>> GetMaster(int id)
        {
            try
            {
                var result = await MasterService.GetMaster(id);
                if (result == null)
                {
                    _response = new MasterResponse
                    {
                        Success = true,
                        Message = "No Master found for pti7!"
                    };
                    return NotFound(_response);
                }

                _response = new MasterResponse
                {
                    Success = true,
                    Master = result
                };
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response = new MasterResponse
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPost]
        public async Task<ActionResult<MastersResponse>> CreateMaster(MasterRequest request)
        {
            try
            {
                if (!await UserService.IsUserAdmin(request.User_Id, "pti7"))
                {
                    return Unauthorized();
                }

                if (request == null)
                {
                    _responses = new MastersResponse()
                    {
                        Success = false,
                        Message = "Request was null",
                    };
                    return BadRequest(_response);
                }

                var result = await MasterService.CreateMaster(request);
                if (result == null || result.Count() == 0)
                {
                    _responses = new MastersResponse
                    {
                        Success = false,
                        Message = "No Master found for pti7!"
                    };
                    return NotFound(_responses);
                }

                _responses = new MastersResponse
                {
                    Success = true,
                    Masters = result
                };
                return CreatedAtAction(nameof(CreateMaster), _responses);
            }
            catch (Exception e)
            {
                _responses = new MastersResponse
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _responses);
            }

        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MastersResponse>> UpdateMaster(int id, MasterRequest request)
        {
            try
            {
                if (!await UserService.IsUserAdmin(request.User_Id, "pti7"))
                {
                    return Unauthorized();
                }

                if (request == null)
                {
                    _responses = new MastersResponse()
                    {
                        Success = false,
                        Message = "Request was null",
                    };
                    return BadRequest(_responses);
                }
                var result = await MasterService.UpdateMaster(id, request);
                if (result == null || result.Count() == 0)
                {
                    return NotFound(new MastersResponse
                    {
                        Success = false,
                        Message = "Master not found"
                    });
                }
                _responses = new MastersResponse
                {
                    Success = true,
                    Masters = result
                };
                return Ok(_responses);
            }
            catch (Exception e)
            {
                _responses = new MastersResponse
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _responses);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteMaster(MastertDeleteRequest request)
        {
            try
            {
                if (!await UserService.IsUserAdmin(request.User_Id, "pti7"))
                {
                    return Unauthorized();
                }
                await MasterService.DeleteMaster(request.Id);
                return Ok(new MastersResponse
                {
                    Success = true,
                    Message = "Master deleted successfully",
                });

            }
            catch (Exception e)
            {
                _responses = new MastersResponse
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _responses);
            }
        }
    }
}
