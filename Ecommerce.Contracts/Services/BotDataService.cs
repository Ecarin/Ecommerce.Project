using Dapper;
using Ecommerce.Contracts.Models.Requests;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Utilities;

namespace Ecommerce.Contracts.Services
{
    public class BotDataService
    {
        public static IEnumerable<BotDataDto> getBotsData()
        {
            return DatabaseUtility.EcommerceConfigurationConnectionString.Query<BotDataDto>("Select * from BotsData where active = 1");
        }

        public static Task<LimitedBotInfo> GetBotData(string shop_name)
        {
            try
            {
                string Query = "Select * from BotsData where shop_name = @shop_name";
                return DatabaseUtility.EcommerceConfigurationConnectionString.QueryFirstOrDefaultAsync<LimitedBotInfo>(Query, new { shop_name });
            }
            catch
            {
                throw;
            }
        }
        public static Task<LimitedBotInfo> UpdateBotData(string shop_name, BotDataRequest request)
        {
            try
            {

                string Query =
                    @"UPDATE [dbo].[BotsData]
                           SET [welcome_message] = @welcome_message
                              ,[about_us] = @about_us
                              ,[contact_us] = @contact_us
                         WHERE shop_name = @shop_name";
                DatabaseUtility.EcommerceConfigurationConnectionString.QueryAsync(Query, new
                {
                    request.welcome_message,
                    request.about_us,
                    request.contact_us,
                    shop_name,
                });
                return GetBotData(shop_name);
            }
            catch
            {
                throw;
            }
        }
    }
}
