using Ecommerce.Contracts.Models.Requests;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Models.Tables.Pti7;
using Ecommerce.Contracts.Services;
using Ecommerce.Contracts.Services.Pti7;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [Route("api/v1/{shop_name}/main_slider")]
    [ApiController]
    public class Main_SliderController : ControllerBase
    {
        Main_SlidersResponse _responses;

        [HttpPost("photo")]
        public async Task<ActionResult> PostPhotoAsync(string shop_name, [FromBody] PhotoRequest request)
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
            string path = $"{shop_name}//main_slider";
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
        public async Task<ActionResult<Main_SlidersResponse>> GetSliderPhotosAsync(string shop_name)
        {
            try
            {
                var result = await Main_SliderService.GetSliderPhotos(shop_name);
                if (result == null || result.Count() == 0)
                {
                    _responses = new Main_SlidersResponse
                    {
                        Success = false,
                        Message = "No slider photo found!"
                    };
                    return NotFound(_responses);
                }

                _responses = new Main_SlidersResponse
                {
                    Success = true,
                    Sliders = result
                };
                return Ok(_responses);
            }
            catch (Exception e)
            {
                _responses = new Main_SlidersResponse
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _responses);
            }
        }


        [HttpPost]
        public async Task<ActionResult<Main_SlidersResponse>> CreateSliderPhotoAsync(string shop_name, Main_SliderRequest request)
        {
            try
            {
                if (!await UserService.IsUserAdmin(request.User_Id, shop_name))
                {
                    return Unauthorized();
                }

                if (request == null)
                {
                    _responses = new Main_SlidersResponse()
                    {
                        Success = false,
                        Message = "Request was null",
                    };
                    return BadRequest(_responses);
                }

                var result = await Main_SliderService.InsertSliderPhotos(shop_name, request);
                if (result == null || result.Count() == 0)
                {
                    _responses = new Main_SlidersResponse
                    {
                        Success = false,
                        Message = "No slider photo found!"
                    };
                    return NotFound(_responses);
                }

                _responses = new Main_SlidersResponse
                {
                    Success = true,
                    Message = "created successfully",
                    Sliders = result
                };
                return Ok(_responses);
            }
            catch (Exception e)
            {
                _responses = new Main_SlidersResponse
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _responses);
            }
        }


        [HttpDelete]
        public async Task<ActionResult> DeleteSliderPhotoAsync(string shop_name, Main_SliderDeleteRequest request)
        {
            try
            {
                if (!await UserService.IsUserAdmin(request.User_Id, shop_name))
                {
                    return Unauthorized();
                }
                await Main_SliderService.DeleteSliderPhoto(shop_name, request.Id);
                return Ok(new Main_SlidersResponse
                {
                    Success = true,
                    Message = "slider photo deleted successfully",
                });

            }
            catch (Exception e)
            {
                _responses = new Main_SlidersResponse
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _responses);
            }
        }
    }
}
