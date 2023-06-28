using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Models.Tables
{
    public class Product_PhotoDto
    {

        public int Photo_Id { get; set; }

        public int Product_Id { get; set; }

        public string Photo_Path { get; set; }
    }
}
