using Dapper;
using Ecommerce.API.Models;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Utilities;
using System.Data.SqlClient;

namespace Ecommerce.Contracts.Services
{
    public class User_AddressService
    {
        public static async Task<AddressDto?> AddAddressAsync(AddressRequest request, string shop_name, string user_id)
        {
            try
            {
                int addressId;
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
                {
                    await _dbConnection.OpenAsync();

                    string query =
                        @"INSERT INTO [user_address] (user_id,country,state,city,street,zipcode)
                        VALUES (@user_id,@country,@state,@city,@street,@zipcode);
                        SELECT SCOPE_IDENTITY() AS addressId;";

                    addressId = await _dbConnection.ExecuteScalarAsync<int>(query, new
                    {
                        user_id,
                        request.country,
                        request.state,
                        request.city,
                        request.street,
                        request.zipcode
                    });
                }
                return (await GetUserAddressesAsync(user_id,shop_name,addressId)).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public static async Task<IEnumerable<AddressDto>> GetUserAddressesAsync(string userId, string shop_name, int? address_id = null)
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
                {
                    await _dbConnection.OpenAsync();

                    string query = 
                        @"SELECT * FROM user_address WHERE
                          (@address_id IS NULL or address_id = @address_id) AND 
                          user_id = @userId and active = 1
                          order by created_at desc";
                    return await _dbConnection.QueryAsync<AddressDto>(query,
                        new
                        {
                            address_id,
                            userId
                        });
                }
            }
            catch
            {
                throw;
            }
        }

        public static async Task<IEnumerable<AddressDto>> DeleteUserAddressesAsync(string user_id, string shop_name, int address_id)
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
                {
                    await _dbConnection.OpenAsync();

                    string query =
                        @"Update user_address Set active = 0 where
                          address_id = @address_id and user_id = @user_id";
                    await _dbConnection.QueryAsync(query,
                        new
                        {
                            address_id,
                            user_id
                        });
                }
                return await GetUserAddressesAsync(user_id, shop_name);
            }
            catch
            {
                throw;
            }
        }

        public static async Task<AddressDto?> UpdateUserAddressAsync(AddressRequest addressRequest, string user_id, int address_id, string shop_name)
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
                {
                    await _dbConnection.OpenAsync();

                    string query =
                        @"UPDATE [user_address]
                            SET 
                            [country] = @country
                            ,[state] = @state
                            ,[city] = @city
                            ,[street] = @street
                            ,[zipcode] = @zipcode
                            ,[updated_at] = GETDATE()
                        WHERE user_id = @user_id and address_id = @address_id";
                    await _dbConnection.QueryAsync(query, new
                    {
                        addressRequest.country,
                        addressRequest.state,
                        addressRequest.city,
                        addressRequest.street,
                        addressRequest.zipcode,
                        user_id,
                        address_id,
                    });
                    return (await GetUserAddressesAsync(user_id, shop_name, address_id)).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
