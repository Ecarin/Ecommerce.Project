using Ecommerce.Contracts.Models.Tables;

namespace Ecommerce.Contracts.Models.Requests
{
    public class CreateOrderRequest
    {
        public string User_Id { get; set; }

        public int User_Address_Id { get; set; }

        public int Shipping_Cost { get; set; }

        public string? Order_Description { get; set; }

        public string Receipt_Photo_Path { get; set; }
    }
    public class UpdateOrderRequest
    {
        public string User_Id { get; set; }
        public string Order_Status { get; set; }
        public string? Tracking_Code { get; set; }

    }
}
