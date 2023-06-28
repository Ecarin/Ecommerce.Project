using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Models.Requests
{
    public class DiscountRequest
    {
        public string user_id { get; set; }
        public string discount_type { get; set; }
        public int discount_value { get; set; }
        public DateTime discount_start_date { get; set; }
        public DateTime discount_end_date { get; set; }
        public int? product_id { get; set; } = null;
        public int? category_id { get; set; } = null;
    }
}
