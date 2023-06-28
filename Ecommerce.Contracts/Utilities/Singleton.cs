using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Models.Utility;
using Ecommerce.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Contracts.Utilities
{
    public class Singleton
    {
        private static Singleton _instance = new Singleton();

        private Singleton()
        {

        }
        public static Singleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Singleton();
                }
                return _instance;
            }
        }

        public DatabaseDto database;

        private IEnumerable<BotDataDto> botsData;

        public IEnumerable<BotDataDto> GetBotsData()
        {
            if (botsData == null)
            {
                botsData = BotDataService.getBotsData();
            }
            return botsData;
        }

        public void SetNewData(IEnumerable<BotDataDto> newData)
        {
            foreach (var oldData in botsData)
            {
                var newBotData = newData.Where(x => x.Id == oldData.Id).First();
                oldData.owner_id = newBotData.owner_id;
                oldData.owner_name = newBotData.owner_name;
                oldData.owner_last_name = newBotData.owner_last_name;
                oldData.owner_phone_number = newBotData.owner_phone_number;
                oldData.owner_national_code = newBotData.owner_national_code;
                oldData.active = newBotData.active;
                oldData.welcome_message = newBotData.welcome_message;
                oldData.user_profile = newBotData.user_profile;
                oldData.user_cart = newBotData.user_cart;
                oldData.user_order = newBotData.user_order;
                oldData.admin_panel = newBotData.admin_panel;
                oldData.about_us = newBotData.about_us;
                oldData.contact_us = newBotData.contact_us;
                oldData.bot_settings = newBotData.bot_settings;
                oldData.special = newBotData.special;
            }
        }

        public DatabaseDto GetDatabaseInfo()
        {
            if (database == null)
            {
                var contents = File.ReadAllText("params.txt").Split(" ");
                database = new DatabaseDto(contents[0], contents[1], contents[2]);
            }
            return database;
        }

    }
}
