using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Models.Tables
{
    public class UserDto
    {

        public string User_Id { get; set; }

        public string Name { get; set; }

        public string Last_Name { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Phone_Number { get; set; }

        public string Role { get; set; }

        public DateTime Created_At { get; set; }

        public DateTime Updated_At { get; set; }

    }
}
