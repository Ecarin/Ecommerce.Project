using Ecommerce.Contracts.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Models.Utility
{
    public class DiscountResult
    {
        public int DiscountedPrice { get; set; }
        public DiscountDto HighestDiscount { get; set; }
    }
}
