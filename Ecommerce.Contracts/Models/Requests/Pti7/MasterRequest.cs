using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Models.Requests.Pti7
{
    public class MasterRequest
    {
        public string User_Id { get; set; }
        public string Name { get; set; }
        public string Last_Name { get; set; }
        public string Description { get; set; }
        public string Photo_Path { get; set; }
    }
    public class MastertDeleteRequest
    {
        public string User_Id { get; set; }
        public int Id { get; set; }
    }
}
