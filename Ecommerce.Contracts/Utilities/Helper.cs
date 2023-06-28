using Ecommerce.Contracts.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Utilities
{
    public class Helper
    {
        public static int GetDiscountedPrice(int price, DiscountDto discount)
        {
            if (discount.Discount_Type.ToLower() == "percent")
            {
                decimal discountedPrice = price - ((discount.Discount_Value * price) / 100);
                return (int)Math.Truncate(discountedPrice);
            }
            else if (discount.Discount_Type.ToLower() == "price")
            {
                int discountedPrice = price - discount.Discount_Value;
                return discountedPrice < 0 ? 0 : discountedPrice;
            }
            else
            {
                return price;
            }
        }

    }
}
