using Ecommerce.Contracts.Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Models.Tables.Pti7
{
    public class MasterResponse : ResponseDto
    {
        public MasterDto? Master { get; set; }
    }
    public class MastersResponse : ResponseDto
    {
        public IEnumerable<MasterDto>? Masters { get; set; }
    }
    public class MasterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Last_Name { get; set; }
        public string Description { get; set;}
        public string Photo_Path { get; set; }
    }
}
