using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Models.Utility
{
    public class DatabaseDto
    {
        public DatabaseDto(string Server, string Username, string Password)
        {
            this.Server = Server;
            this.Username = Username;
            this.Password = Password;
        }

        public string Server { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
    }
}
