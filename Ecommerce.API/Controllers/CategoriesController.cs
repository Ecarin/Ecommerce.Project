using Ecommerce.Contracts.Models.Requests;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/v1/{shop_name}/categories")]
    public class CategoriesController : ControllerBase
    {
        CategoriesResponse _responses;

        [HttpPost]
        public async Task<ActionResult<CategoryResponse>> CreateCategory(string shop_name, [FromBody] CategoryRequest request)
        {
            try
            {
                if (request == null)
                {
                    _responses = new CategoriesResponse()
                    {
                        Success = false,
                        Message = "request can not be null"
                    };
                    return BadRequest(_responses);
                }

                if (await UserService.IsUserAdmin(request.user_id, shop_name) == false)
                {
                    _responses = new CategoriesResponse()
                    {
                        Success = false,
                        Message = "only admin can create category!"
                    };
                    return Unauthorized(_responses);
                }

                var result = await CategoryService.CreateCategoryAsync(request, shop_name);
                if (result.Count() == 0 || result == null)
                {
                    _responses = new CategoriesResponse()
                    {
                        Success = false,
                        Message = "failed to create category!"
                    };
                    return StatusCode(StatusCodes.Status500InternalServerError, _responses);
                }

                _responses = new CategoriesResponse()
                {
                    Success = true,
                    Message = "Success!",
                    Categories = result
                };
                return CreatedAtAction(nameof(CreateCategory), _responses);

            }
            catch (Exception e)
            {
                _responses = new CategoriesResponse()
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _responses);
            }
        }


        [HttpPut("{category_id}")]
        public async Task<ActionResult<CategoriesResponse>> UpdateCategory(string shop_name, int category_id, [FromBody] CategoryRequest request)
        {
            try
            {
                if (request == null)
                {
                    _responses = new CategoriesResponse()
                    {
                        Success = false,
                        Message = "request can not be null"
                    };
                    return BadRequest(_responses);
                }

                if (await UserService.IsUserAdmin(request.user_id, shop_name) == false)
                {
                    _responses = new CategoriesResponse()
                    {
                        Success = false,
                        Message = "only admin can create category!"
                    };
                    return Unauthorized(_responses);
                }

                var result = await CategoryService.UpdateCategoryAsync(request, category_id, shop_name);
                if (result.Count() == 0 || result == null)
                {
                    _responses = new CategoriesResponse()
                    {
                        Success = false,
                        Message = "no categories found!"
                    };
                    return StatusCode(StatusCodes.Status404NotFound, _responses);
                }

                _responses = new CategoriesResponse()
                {
                    Success = true,
                    Message = "Success!",
                    Categories = result
                };
                return CreatedAtAction(nameof(UpdateCategory), _responses);
            }
            catch (Exception e)
            {
                _responses = new CategoriesResponse()
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _responses);
            }
        }

        [HttpDelete("{category_id}")]
        public async Task<ActionResult<CategoriesResponse>> DeleteCategory(string shop_name, string user_id, int category_id)
        {
            try
            {
                if (await UserService.IsUserAdmin(user_id, shop_name) == false)
                {
                    _responses = new CategoriesResponse()
                    {
                        Success = false,
                        Message = "only admin can create category!"
                    };
                    return Unauthorized(_responses);
                }

                var result = await CategoryService.DeleteCategoryAsync(shop_name, category_id);
                if (result.Count() == 0 || result == null)
                {
                    _responses = new CategoriesResponse()
                    {
                        Success = false,
                        Message = "no categories found!"
                    };
                    return StatusCode(StatusCodes.Status404NotFound, _responses);
                }

                _responses = new CategoriesResponse()
                {
                    Success = true,
                    Message = "Success!",
                    Categories = result
                };
                return CreatedAtAction(nameof(DeleteCategory), _responses);
            }
            catch (Exception e)
            {
                _responses = new CategoriesResponse()
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _responses);
            }
        }

        [HttpGet]
        public async Task<ActionResult<CategoriesResponse>> GetCategories(string shop_name, int? category_id = null)
        {
            try
            {
                var result = await CategoryService.GetCategoriesAsync(shop_name, category_id);
                if (result.Count() == 0 || result == null)
                {
                    _responses = new CategoriesResponse()
                    {
                        Success = false,
                        Message = "no categories found!"
                    };
                    return StatusCode(StatusCodes.Status404NotFound, _responses);
                }

                _responses = new CategoriesResponse()
                {
                    Success = true,
                    Message = "Success!",
                    Categories = result
                };
                return CreatedAtAction(nameof(GetCategories), _responses);
            }
            catch (Exception e)
            {
                _responses = new CategoriesResponse()
                {
                    Success = false,
                    Message = $"{e.HResult}: {e.Message}",
                };
                return StatusCode(StatusCodes.Status500InternalServerError, _responses);
            }
        }
    }
}