using Dapper;
using Ecommerce.Contracts.Models.Requests.Pti7;
using Ecommerce.Contracts.Models.Tables.Pti7;
using Ecommerce.Contracts.Utilities;
using System.Data.SqlClient;

namespace Ecommerce.Contracts.Services.Pti7
{
    public class MasterService
    {
        public static async Task<MasterDto?> GetMaster(int id)
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString("pti7"))
                {
                    await _dbConnection.OpenAsync();
                    string Query = "select * from masters where id = @id";
                    return await _dbConnection.QueryFirstOrDefaultAsync<MasterDto>(Query, new { id });
                }
            }
            catch
            {
                throw;
            }
        }

        public static async Task<IEnumerable<MasterDto>?> GetMasters()
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString("pti7"))
                {
                    await _dbConnection.OpenAsync();
                    string Query = "select * from masters";
                    return await _dbConnection.QueryAsync<MasterDto>(Query);
                }
            }
            catch
            {
                throw;
            }
        }

        public static async Task<IEnumerable<MasterDto>?> CreateMaster(MasterRequest request)
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString("pti7"))
                {
                    await _dbConnection.OpenAsync();
                    string Query = @"INSERT INTO [dbo].[Masters]
                                       ([name]
                                       ,[last_name]
                                       ,[description]
                                       ,[photo_path])
                                 VALUES
                                       (@Name
                                       ,@Last_Name
                                       ,@Description
                                       ,@Photo_Path)";
                     await _dbConnection.ExecuteAsync(Query, new
                    {
                        request.Name,
                        request.Last_Name,
                        request.Description,
                        request.Photo_Path
                    });
                    return await GetMasters();
                }
            }
            catch
            {
                throw;
            }
        }

        public static async Task<IEnumerable<MasterDto>?> UpdateMaster(int id, MasterRequest request)
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString("pti7"))
                {
                    await _dbConnection.OpenAsync();
                    string Query = @"UPDATE [dbo].[Masters]
                       SET [name] = @Name
                          ,[last_name] = @Last_Name
                          ,[description] = @Description
                          ,[photo_path] = @Photo_Path
                        Where id = @id";
                    await _dbConnection.ExecuteAsync(Query, new
                    {
                        request.Name,
                        request.Last_Name,
                        request.Description,
                        request.Photo_Path,
                        id
                    });
                    return await GetMasters();
                }
            }
            catch
            {
                throw;
            }
        }

        public static async Task<string> DeleteMaster(int Id)
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString("pti7"))
                {
                    await _dbConnection.OpenAsync();
                    string Query = "delete from masters where id = @Id";
                    await _dbConnection.QueryAsync(Query, new {Id});
                    return "master removed successfully";
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
