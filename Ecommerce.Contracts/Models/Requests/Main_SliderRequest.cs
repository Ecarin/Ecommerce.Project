using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Models.Requests
{
    public class Main_SliderRequest
    {
        public string User_Id { get; set; }
        public string Photo_Path { get; set; }
        public string Url { get; set; }
    }
    public class Main_SliderDeleteRequest
    {
        public string User_Id { get; set; }
        public int Id { get; set; }
    }
}
