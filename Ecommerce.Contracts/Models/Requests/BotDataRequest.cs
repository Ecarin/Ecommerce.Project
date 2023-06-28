using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Ecommerce.Contracts.Models.Requests
{
    public class BotDataRequest
    {
        public string user_id { get; set; }
        public string welcome_message { get; set; }
        public string about_us { get; set; }
        public string contact_us { get; set; }
    }
}
