using Ecommerce.Contracts.Models.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Models.Tables
{
    public class OrderResponse : ResponseDto
    {
        [NotMapped]
        public OrderDto? Order { get; set; }
    }
    public class OrdersResponse : PaginatedResponse
    {
        [NotMapped]
        public IEnumerable<OrderDto>? Orders { get; set; }
    }
    public class OrderDto
    {
        public int Order_Id { get; set; }

        public string User_Id { get; set; }

        public string User_Full_Name { get; set; }

        public int User_Address_Id { get; set; }

        public string? Full_Address { get; set; }

        public int shipping_cost { get; set; }

        public int Total_Price { get; set; }

        public string? Order_Description { get; set; }

        public string Receipt_Photo { get; set; }

        public string Order_Status { get; set; }

        public string? Tracking_Code { get; set; }

        public DateTime Order_Date { get; set; }

        public DateTime? Updated_At { get; set; }

        public List<Order_ItemDto> Order_Items { get; set; }
    }
}
