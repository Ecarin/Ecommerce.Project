using Dapper;
using Ecommerce.Contracts.Models.Requests;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Utilities;
using System.Data.SqlClient;

namespace Ecommerce.Contracts.Services
{
    public class Main_SliderService
    {
        public static async Task<IEnumerable<Main_SliderDto>?> GetSliderPhotos(string shop_name)
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
                {
                    await _dbConnection.OpenAsync();
                    string Query = "select * from main_slider";
                    return await _dbConnection.QueryAsync<Main_SliderDto>(Query);
                }
            }
            catch
            {
                throw;
            }
        }


        public static async Task<IEnumerable<Main_SliderDto>?> InsertSliderPhotos(string shop_name, Main_SliderRequest request)
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
                {
                    await _dbConnection.OpenAsync();
                    string Query = @"INSERT INTO [dbo].[main_slider]
                                       ([url]
                                       ,[photo_path])
                                 VALUES
                                       (@Url
                                       ,@Photo_Path)";
                    await _dbConnection.ExecuteAsync(Query, new
                    {
                        request.Url,
                        request.Photo_Path,
                    });
                    return await GetSliderPhotos(shop_name);
                }
            }
            catch
            {
                throw;
            }
        }

        public static async Task<string> DeleteSliderPhoto(string shop_name, int Id)
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
                {
                    await _dbConnection.OpenAsync();
                    string Query = "delete from main_slider where id = @Id";
                    await _dbConnection.QueryAsync(Query, new { Id });
                    return "slider photo removed successfully";
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
