using Ecommerce.Contracts.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [Route("api/v1/{shop_name}/receipt_photos")]
    [ApiController]
    public class ReceiptPhotosController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> PostPhoto([FromBody] PhotoRequest request, string shop_name)
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
            string path = $"{shop_name}//receipts";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Save the photo to disk.
            await System.IO.File.WriteAllBytesAsync($"{path}//{filename}", bytes);

            // Return a response containing path.
            return Ok($"{path}//{filename}");
        }
    }
}
