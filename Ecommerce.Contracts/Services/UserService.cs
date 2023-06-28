using Dapper;
using Ecommerce.API.Models;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Utilities;
using System.Data.SqlClient;

namespace Ecommerce.Contracts.Services
{
    public class UserService
    {
        public static async Task<int> UpsertUserAsync(UserRequest user, string user_id, string shop_name)
        {
            int affectedRows = 0;
            using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
            {
                try
                {
                    await _dbConnection.OpenAsync();
                    string Query =
                        @"MERGE users AS target
                    USING (VALUES (@user_id, @name, @last_name, @username, @email,
                                   @phone_number, 'user', GETDATE(), GETDATE())) AS source (user_id, name, last_name, username, email, 
                                                                                          phone_number, role, created_at, updated_at)
                    ON target.user_id = source.user_id
                    WHEN MATCHED THEN
                        UPDATE SET name = source.name,
                                   last_name = source.last_name,
                                   username = source.username,
                                   email = source.email,
                                   phone_number = source.phone_number,
                                   updated_at = source.updated_at
                    WHEN NOT MATCHED THEN
                        INSERT (user_id, name, last_name, username, email, phone_number, role, created_at, updated_at)
                        VALUES (source.user_id, source.name, source.last_name, source.username, source.email, 
                                source.phone_number, source.role, source.created_at, source.updated_at);";
                    affectedRows = await _dbConnection.ExecuteAsync(Query, new { user_id, user.name, user.last_name, user.username, user.email, user.phone_number} );
                }
                catch (Exception ex)
                {
                    //_logger.LogError(ex, $"Failed to upsert user with Id [{user.user_id}] to [{shopName}].");
                }
            }
            return affectedRows;
        }

        public static async Task<UserDto> GetUserAsync(string userId, string shop_name)
        {
            using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
            {
                try
                {
                    await _dbConnection.OpenAsync();
                    string Query =
                        @"SELECT * FROM users WHERE user_id = @userId;";
                    return await _dbConnection.QueryFirstOrDefaultAsync<UserDto>(Query, new { userId });
                }
                catch (Exception ex)
                {
                    //_logger.LogError(ex, $"Failed to get user with Id [{userId}] from [{shopName}].");
                    return null;
                }
            }
        }

        public static async Task<bool> IsUserAdmin(string user_id, string shop_name)
        {
            using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
            {
                try
                {
                    await _dbConnection.OpenAsync();
                    string query = "SELECT role FROM users WHERE user_id = @user_id";
                    string role = await _dbConnection.ExecuteScalarAsync<string>(query, new { user_id });
                    if (role.ToLower().Equals("admin"))
                        return true;
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    //_logger.LogError(ex, $"Failed to check if user with Id [{user_id}] is admin.");
                    return false;
                }
            }
        }
    }
}
