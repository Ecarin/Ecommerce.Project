using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Models.Requests
{
    public class ProductRequest
    {
        public string user_id { get; set; }
        public string product_name { get; set; }
        public string description { get; set; }
        public int price { get; set; }
        public int quantity { get; set; }
        public List<int> category_ids { get; set; }
        public List<string> photos { get; set; }
    }
    public class ProductDeleteRequest
    {
        public string user_id { get; set; }
        public int product_id { get; set; }
    }
}
