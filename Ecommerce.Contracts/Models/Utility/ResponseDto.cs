using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Models.Utility
{
    public class ResponseDto
    {
        [NotMapped]
        public bool Success { get; set; }
        [NotMapped]
        public string Message { get; set; }
    }
    public class PaginatedResponse
    {
        [NotMapped]
        public int Page { get; set; }

        [NotMapped]
        public int Limit { get; set; }

        [NotMapped]
        public int TotalRows { get; set; }
    }
}
