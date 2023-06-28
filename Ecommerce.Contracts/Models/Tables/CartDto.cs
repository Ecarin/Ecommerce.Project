using Ecommerce.Contracts.Models.Requests;
using Ecommerce.Contracts.Models.Utility;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Contracts.Models.Tables
{
    public class CartResponse : ResponseDto
    {
        [NotMapped]
        public CartDto? Cart { get; set; }
    }

    public class CartDto
    {

        public int Cart_Id { get; set; }

        public string User_Id { get; set; }


        public DateTime Upserted_At { get; set; }

        [NotMapped]
        public int TotalPrice { get; set; }

        [NotMapped]
        public IEnumerable<CartItemDto> CartItems { get; set; }
    }

    public class CartItemDto
    {

        public int Cart_Item_Id { get; set; }

        public int Cart_Id { get; set; }

        public int Product_Id { get; set; }

        public string Product_Name { get; set; }

        public int Quantity { get; set; }

        [NotMapped]
        public int Price { get; set; }

        [NotMapped]
        public int DiscountedPrice { get; set; }
    }
}
