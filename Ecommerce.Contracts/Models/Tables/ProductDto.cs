using Ecommerce.Contracts.Models.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Models.Tables
{
    public class ProductsResponse : PaginatedResponse
    {
        public IEnumerable<ProductsDto> Products { get; set; }
    }
    public class ProductDto
    {

        public int Product_Id { get; set; }

        public string Product_Name { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }

        public int Quantity { get; set; }

        [NotMapped]
        public int DiscountedPrice { get; set; }

        [NotMapped]
        public List<int> CategoryIds { get; set; }

        [NotMapped]
        public List<string> Photos { get; set; }

        [NotMapped]
        public DiscountDto? Discount { get; set; }
    }

    public class ProductsDto
    {

        public int Product_Id { get; set; }

        public string Product_Name { get; set; }

        public int Price { get; set; }

        public int DiscountedPrice { get; set; }

        public int Quantity { get; set; }

        public string Photo_path { get; set; }

        public DateTime Updated_At { get; set; }
    }
}
