using Ecommerce.Contracts.Models.Utility;
using Telegram.Bot;

namespace Ecommerce.Contracts.Models.Tables
{
    public class BotDataResponse : ResponseDto
    {
        public LimitedBotInfo BotData { get; set; }
    }
    public class LimitedBotInfo
    {
        public string owner_name { get; set; }
        public string owner_last_name { get; set; }
        public string owner_phone_number { get; set; }
        public string shop_name { get; set; }
        public string bot_username { get; set; }
        public string welcome_message { get; set; }
        public string about_us { get; set; }
        public string contact_us { get; set; }
        public bool special { get; set; }
        public bool active { get; set; }
    }
    public class BotDataDto
    {
        public TelegramBotClient? bot { get; set; }
        public int Id { get; set; }
        public string owner_id { get; set; }
        public string owner_name { get; set; }
        public string owner_last_name { get; set; }
        public string owner_phone_number { get; set; }
        public string owner_national_code { get; set; }
        public string shop_name { get; set; }
        public string bot_username { get; set; }
        public string bot_token { get; set; }
        public bool active { get; set; }
        public string welcome_message { get; set; }
        public string user_profile { get; set; }
        public string user_cart { get; set; }
        public string user_order { get; set; }
        public string admin_panel { get; set; }
        public string about_us { get; set; }
        public string contact_us { get; set; }
        public string bot_settings { get; set; }
        public bool special { get; set; }
    }
}
