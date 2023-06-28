using Ecommerce.Contracts.Models.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Models.Tables
{
    public class CategoryResponse : ResponseDto
    {
        [NotMapped]
        public CategoryDto Category { get; set; }
    }
    public class CategoriesResponse : ResponseDto
    {
        [NotMapped]
        public IEnumerable<CategoryDto> Categories { get; set; }
    }
    public class CategoryDto
    {
        public int? Category_Id { get; set; }

        public string Category_Name { get; set; }

        public int? Parent_Id { get; set; }

        [NotMapped]
        public DiscountDto? Discount { get; set; }

        [NotMapped]
        public List<CategoryDto> Children { get; set; }
    }
}
