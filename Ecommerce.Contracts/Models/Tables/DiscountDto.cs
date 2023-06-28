using Ecommerce.Contracts.Models.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Models.Tables
{
    public class DiscountResponse : ResponseDto
    {
        [NotMapped]
        public DiscountDto Discount { get; set; }
    }
    public class DiscountsResponse : ResponseDto
    {
        [NotMapped]
        public IEnumerable<DiscountDto> Discounts { get; set; }
    }

    public class DiscountDto
    {

        public int Discount_Id { get; set; }

        public string Discount_Type { get; set; }

        public int Discount_Value { get; set; }

        public DateTime Discount_Start_Date { get; set; }

        public DateTime Discount_End_Date { get; set; }

        public int? Product_Id { get; set; }

        public int? Category_Id { get; set; }
    }
}
