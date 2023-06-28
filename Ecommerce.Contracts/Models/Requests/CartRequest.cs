namespace Ecommerce.Contracts.Models.Requests
{
    public class CartRequest
    {
        public string user_id { get; set; }
        public List<CartItemRequest> cart_items { get; set; }
    }

    public class CartItemRequest
    {
        public int product_id { get; set; }
        public int quantity { get; set; }
    }
}
