using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Models.Requests
{
    public class CategoryRequest
    {
        public string user_id { get; set; }
        public string category_name { get; set; }
        public int? parent_id { get; set; }
    }
}
