using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Models.Tables
{
    public class Product_CategoryDto
    {

        public int Product_Id { get; set; }

        public int Category_Id { get; set; }
    }
}
