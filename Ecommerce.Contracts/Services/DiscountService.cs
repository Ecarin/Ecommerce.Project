using Dapper;
using Ecommerce.Contracts.Models.Requests;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Utilities;
using System.Data.SqlClient;

namespace Ecommerce.Contracts.Services
{
    public class DiscountService
    {
        public static async Task<IEnumerable<DiscountDto>> GetDiscountsAsync(string shopName, int? discount_id = null)
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shopName))
                {
                    await _dbConnection.OpenAsync();

                    string query =
                        @"SELECT [discount_id]
                       ,[discount_type]
                       ,[discount_value]
                       ,[discount_start_date]
                       ,[discount_end_date]
                       ,[product_id]
                       ,[category_id]
                        FROM [discounts]
                        WHERE (@discount_id IS NULL or discount_id = @discount_id)";
                    return await _dbConnection.QueryAsync<DiscountDto>(query, new { discount_id });
                }
            }
            catch
            {
                throw;
            }
        }

        public static async Task<DiscountDto> CreateDiscountAsync(DiscountRequest discountRequest, string shop_name)
        {
            try
            {
                int discount_id;
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
                {
                    await _dbConnection.OpenAsync();

                    string query =
                        @"INSERT INTO [discounts]
                           ([discount_type]
                           ,[discount_value]
                           ,[discount_start_date]
                           ,[discount_end_date]
                           ,[product_id]
                           ,[category_id])
                        VALUES
                           (@discount_type
                           ,@discount_value
                           ,@discount_start_date
                           ,@discount_end_date
                           ,@product_id
                           ,@category_id);
                        SELECT SCOPE_IDENTITY() AS discount_id;";
                    discount_id = await _dbConnection.ExecuteScalarAsync<int>(query, discountRequest);
                }
                return (await GetDiscountsAsync(shop_name, discount_id)).First();
            }
            catch
            {
                throw;
            }
        }


        public static async Task<DiscountDto> UpdateDiscountAsync(int discountId, DiscountRequest discountRequest, string shop_name)
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
                {
                    await _dbConnection.OpenAsync();

                    string query =
                        @"UPDATE [discounts] SET 
                            [discount_type] = @discount_type,
                            [discount_value] = @discount_value,
                            [discount_start_date] = @discount_start_date,
                            [discount_end_date] = @discount_end_date,
                            [product_id] = @product_id,
                            [category_id] = @category_id
                        WHERE [discount_id] = @discount_id";
                    await _dbConnection.ExecuteAsync(query, new
                    {
                        discountRequest.discount_type,
                        discountRequest.discount_value,
                        discountRequest.discount_start_date,
                        discountRequest.discount_end_date,
                        discountRequest.product_id,
                        discountRequest.category_id,
                        discount_id = discountId
                    });
                }
                return (await GetDiscountsAsync(shop_name, discountId)).First();
            }
            catch
            {
                throw;
            }
        }


        public static async Task<IEnumerable<DiscountDto>> DeleteDiscountAsync(int discountId, string shop_name)
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
                {
                    await _dbConnection.OpenAsync();

                    string query = "DELETE FROM [discounts] WHERE [discount_id] = @discountId";
                    await _dbConnection.ExecuteAsync(query, new { discountId });
                }
                return await GetDiscountsAsync(shop_name);
            }
            catch
            {
                throw;
            }
        }

    }
}
