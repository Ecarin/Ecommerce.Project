using Ecommerce.Contracts.Models.Requests;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/v1/{shop_name}/orders")]
    public class OrdersController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<OrderResponse>> CreateOrder([FromBody] CreateOrderRequest request, string shop_name)
        {
            var result = await OrderService.CreateOrderAsync(shop_name, request);
            if (result == null)
            {
                return BadRequest(new OrderResponse
                {
                    Success = false,
                    Message = "Failed to create order!"
                });
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpGet("{order_id}")]
        public async Task<ActionResult<OrderResponse>> GetOrderById(string shop_name, int order_id)
        {
            var order = await OrderService.GetOrderByIdAsync(shop_name, order_id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpGet]
        public async Task<ActionResult<OrdersResponse>> GetOrders(string shop_name, int? orderId = null, string? userFullName = "", string sortBy = "order_date", string order = "desc", int? totalPrice = null, string? orderStatus = null, int limit = 20, int page = 1)
        {
            try
            {
                var ordersResponse = await OrderService.GetOrdersAsync(shop_name, orderId, userFullName, sortBy, order, totalPrice, orderStatus, limit, page);
                return Ok(ordersResponse);
            }
            catch (Exception ex)
            {
                // Log the error and return an error response
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("user/{user_id}")]
        public async Task<ActionResult<OrderResponse>> GetOrdersByUserId(string shop_name, string user_id, string sortBy = "order_date", string order = "asc", string? order_status = null)
        {
            try
            {
                var result = await OrderService.GetOrdersByUserIdAsync(shop_name, user_id, sortBy, order, order_status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the error and return an error response
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{orderId}")]
        public async Task<ActionResult<OrderResponse>> UpdateOrderStatus(string shop_name, int orderId, [FromBody] UpdateOrderRequest request)
        {
            try
            {
                if (!await UserService.IsUserAdmin(request.User_Id, shop_name))
                {
                    return Unauthorized();
                }
                // Call the UpdateOrderStatusAsync method to update the order
                OrderResponse orderResponse = await OrderService.UpdateOrderStatusAsync(shop_name, request, orderId);

                // Return the updated order as a response
                return Ok(orderResponse);
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error with the exception message if an error occurred
                return StatusCode(500, ex.Message);
            }
        }

    }
}
