using Ecommerce.API.Models;
using Ecommerce.Contracts.Services;
using Ecommerce.Contracts.Models.Tables;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/{shop_name}/users")]
public class UsersController : ControllerBase
{
    [HttpPut("{user_id}")]
    public async Task<ActionResult> UpsertUser(string shop_name, string user_id, [FromBody] UserRequest usersRequest)
    {
        if (usersRequest == null)
        {
            return BadRequest("User data is missing or invalid");
        }

        await UserService.UpsertUserAsync(usersRequest,user_id, shop_name);

        return Ok();
    }

    [HttpGet("{user_id}/GetUser")]
    public async Task<ActionResult<UserDto>> GetUser(string shop_name, string user_id)
    {
        var user = await UserService.GetUserAsync(user_id, shop_name);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
}
