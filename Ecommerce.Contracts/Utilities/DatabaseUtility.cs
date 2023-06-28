using Dapper;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Models.Utility;
using System.Data.SqlClient;

namespace Ecommerce.Contracts.Utilities
{
    public class DatabaseUtility
    {
        public static string GetConnectionString(string databaseName)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(); ;
            builder.DataSource = Singleton.Instance.GetDatabaseInfo().Server;
            builder.InitialCatalog = databaseName;
            builder.UserID = Singleton.Instance.GetDatabaseInfo().Username;
            builder.Password = Singleton.Instance.GetDatabaseInfo().Password;

            return builder.ConnectionString;
        }
        public static SqlConnection EcommerceConfigurationConnectionString = new SqlConnection(GetConnectionString("EcommerceConfiguration"));
        public static SqlConnection ShopConnectionString(string shop_name)
        {
            return new SqlConnection(GetConnectionString(shop_name));
        }
        public static async Task<DiscountResult?> calculateBestDiscount(SqlConnection connection, int product_id, int product_price, List<int>? categoryIds = null)
        {
            try
            {

                if (categoryIds == null)
                {
                    string categoryIdsQuery =@"
					SELECT category_id
					FROM product_categories
					WHERE product_id = @product_id";
                    categoryIds = (await connection.QueryAsync<int>(categoryIdsQuery, new { product_id })).ToList();
                }

                string discountsQuery = "SELECT * FROM Discounts WHERE Product_Id = @product_id or Category_Id IN @categoryIds";
                var discounts = await connection.QueryAsync<DiscountDto>(discountsQuery, new { product_id, categoryIds });
                if (discounts != null && discounts.Count() > 0)
                {
                    List<(int, DiscountDto)> discountWithPrice = new List<(int, DiscountDto)>();
                    foreach (var discount in discounts)
                    {
                        if (discount.Discount_Start_Date <= DateTime.Today
                            && discount.Discount_End_Date >= DateTime.Today)
                        {
                            var price = Helper.GetDiscountedPrice(product_price, discount);
                            discountWithPrice.Add((price, discount));
                        }
                        else
                        {
                            discountWithPrice.Add((product_price, discount));
                        }
                    }

                    (int lowestPrice, DiscountDto highestDiscount) = discountWithPrice.OrderBy(d => d.Item1).First();
                    return new DiscountResult()
                    {
                        DiscountedPrice = lowestPrice,
                        HighestDiscount = highestDiscount
                    };
                }
                return new DiscountResult()
                {
                    DiscountedPrice = product_price,
                    HighestDiscount = null
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static async Task<List<int>> GetValidCategories(SqlConnection connection, List<int> CategoryIds)
        {
            List<int>? valid_categories = new List<int>();
            string activeCategories =
    @"WITH category_tree (category_id, category_name, parent_id, level, active) AS (
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
SELECT category_id
FROM category_tree
ORDER BY level, category_id;";

            foreach (var item in CategoryIds)
            {
                var result = (await connection.QueryAsync<int>(activeCategories, new { CategoryId = item })).ToList();
                if (result != null)
                    valid_categories.AddRange(result);
            }
            return valid_categories.Distinct().ToList();
        }
    }
}
