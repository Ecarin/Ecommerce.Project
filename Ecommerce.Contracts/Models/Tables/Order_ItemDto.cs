using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Models.Tables
{
    public class Order_ItemDto
    {
        public int Order_Item_Id { get; set; }

        public int Order_Id { get; set; }

        public int Product_Id { get; set; }

        public string Product_Name { get; set; }

        public int Tag_Price { get; set; }

        public int Final_Price { get; set; }

        public int Quantity { get; set; }
    }
}
