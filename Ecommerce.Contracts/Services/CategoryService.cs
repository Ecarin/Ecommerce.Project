using Dapper;
using Ecommerce.Contracts.Models.Requests;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Utilities;
using System.Data;
using System.Data.SqlClient;

namespace Ecommerce.Contracts.Services
{
    public class CategoryService
    {
        public static async Task<IEnumerable<CategoryDto>> CreateCategoryAsync(CategoryRequest request, string shop_name)
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
                {
                    await _dbConnection.OpenAsync();

                    if (request.parent_id != 0 && request.parent_id != null)
                    {
                        //check if parent_id exists to be assinged
                        int parent_count = _dbConnection.ExecuteScalar<int>("select count(*) from categories where category_id = @parent_id and active = 1", new { request.parent_id });
                        if (parent_count == 0)
                            return null;
                    }
                    string query =
                        @"INSERT INTO [categories]
                               ([category_name]
                               ,[parent_id])
                         VALUES
                               (@category_name
                               ,@parent_id)";
                    await _dbConnection.ExecuteAsync(query, request);
                }
                return await GetCategoriesAsync(shop_name);
            }
            catch
            {
                throw;
            }
        }

        public static async Task<IEnumerable<CategoryDto>> UpdateCategoryAsync(CategoryRequest request, int category_id, string shop_name)
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
                {
                    await _dbConnection.OpenAsync();

                    if (request.parent_id != 0 && request.parent_id != null)
                    {
                        //check if parent_id exists to be assinged
                        int parent_count = _dbConnection.ExecuteScalar<int>("select count(*) from categories where category_id = @parent_id and active = 1", new { request.parent_id });
                        if (parent_count == 0)
                            return null;
                    }
                    string query =
                        @"UPDATE [dbo].[categories]
                           SET [category_name] = @category_name
                              ,[parent_id] = @parent_id
                         WHERE category_id = @category_id";
                    await _dbConnection.ExecuteAsync(query, new { category_id, request.category_name, request.parent_id });
                }
                return await GetCategoriesAsync(shop_name, category_id);
            }
            catch
            {
                throw;
            }
        }

        public static async Task<IEnumerable<CategoryDto>> DeleteCategoryAsync(string shop_name, int category_id)
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
                {
                    await _dbConnection.OpenAsync();

                    string query =
                        @"update [dbo].[categories] set active = 0
                            WHERE category_id = @category_id";
                    await _dbConnection.ExecuteAsync(query, new { category_id });
                }
                return await GetCategoriesAsync(shop_name);
            }
            catch
            {
                throw;
            }
        }

        public static async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(string shop_name, int? categoryId = null)
        {
            try
            {
                var rootCategories = new List<CategoryDto>();
                var categoryMap = new Dictionary<int?, CategoryDto>();
                // Execute the SQL query to retrieve categories and their children from the given category ID, if it exists
                // Otherwise, retrieve all categories and their children
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
                {
                    await _dbConnection.OpenAsync();

                    var sql = @"
WITH category_tree (category_id, category_name, parent_id, level, active) AS (
    SELECT category_id, category_name, parent_id, 0 AS level, active
    FROM categories
    WHERE ((@CategoryId IS NULL AND parent_id IS NULL) OR
         (@CategoryId IS NOT NULL AND category_id = @CategoryId))
         AND active = 1 
    UNION ALL
    SELECT c.category_id, c.category_name, c.parent_id, ct.level + 1, c.active
    FROM categories c
    JOIN category_tree ct ON c.parent_id = ct.category_id
	where c.active = 1
)
SELECT category_id, category_name, parent_id
FROM category_tree
ORDER BY level, category_id;
                    ";

                    var param = new DynamicParameters();
                    param.Add("@CategoryId", categoryId, DbType.Int32, ParameterDirection.Input);

                    var categories = await _dbConnection.QueryAsync<CategoryDto>(sql, param);

                    // Build up the tree structure
                    foreach (var category in categories)
                    {
                        DiscountDto? discount = null;
                        if (category.Category_Id != null)
                        {
                            string getDiscountQuery = "select top(1) * from discounts where category_id = @category_id";
                            discount = await _dbConnection.QueryFirstOrDefaultAsync<DiscountDto>(getDiscountQuery, new { category_id = category.Category_Id });
                        }
                     
                        if (!categoryMap.TryGetValue(category.Category_Id, out var categoryDto))
                        {
                            categoryDto = new CategoryDto
                            {
                                Category_Id = category.Category_Id,
                                Category_Name = category.Category_Name,
                                Parent_Id = category.Parent_Id,
                                Discount = discount,
                                Children = new List<CategoryDto>()
                            };
                            categoryMap.Add(category.Category_Id, categoryDto);
                        }

                        int? parent_id = null;
                        if (categoryId != null)
                        {
                            parent_id = _dbConnection.ExecuteScalar<int?>($"select parent_id from categories where category_id = {categoryId} and active = 1");
                        }
                        if (category.Parent_Id == parent_id)
                        {
                            rootCategories.Add(categoryDto);
                        }
                        else
                        {
                            if (!categoryMap.TryGetValue(category.Parent_Id, out var parentDto))
                            {
                                parentDto = new CategoryDto
                                {
                                    Category_Id = category.Parent_Id,
                                    Children = new List<CategoryDto>()
                                };
                                categoryMap.Add(category.Parent_Id, parentDto);
                            }
                            parentDto.Children.Add(categoryDto);
                        }
                    }
                }
                return rootCategories;
            }
            catch
            {
                throw;
            }
        }
    }
}
