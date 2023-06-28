using Dapper;
using Ecommerce.Contracts.Models.Requests;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Utilities;
using System.Data.SqlClient;

namespace Ecommerce.Contracts.Services
{
    public class ProductService
    {
        public static async Task<ProductDto> CreateProductAsync(ProductRequest product, string shopName)
        {
            // Create a new SqlConnection object
            using (SqlConnection connection = DatabaseUtility.ShopConnectionString(shopName))
            {
                // Open the database connection
                await connection.OpenAsync();

                // Begin a new transaction
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert a new record into the products table
                        string insertProductQuery = @"
                            INSERT INTO products (product_name, description, price, quantity)
                            VALUES (@product_name, @description, @price, @quantity);
                            SELECT SCOPE_IDENTITY();";

                        using (SqlCommand command = new SqlCommand(insertProductQuery, connection, transaction))
                        {
                            // Add parameters to the command
                            command.Parameters.AddWithValue("@product_name", product.product_name);
                            command.Parameters.AddWithValue("@description", product.description);
                            command.Parameters.AddWithValue("@price", product.price);
                            command.Parameters.AddWithValue("@quantity", product.quantity);

                            // Execute the insert query and get the newly created product ID
                            int product_Id = Convert.ToInt32(await command.ExecuteScalarAsync());

                            // Insert records into the product_categories table for each category ID in the product object
                            if (product.category_ids != null && product.category_ids.Any())
                            {
                                string insertProductCategoryQuery = @"
                                    INSERT INTO product_categories (product_id, category_id)
                                    VALUES (@product_Id, @category_Id);";

                                foreach (int category_Id in product.category_ids)
                                {
                                    command.Parameters.Clear();
                                    command.CommandText = insertProductCategoryQuery;
                                    command.Parameters.AddWithValue("@product_Id", product_Id);
                                    command.Parameters.AddWithValue("@category_Id", category_Id);
                                    await command.ExecuteNonQueryAsync();
                                }
                            }

                            // Insert records into the product_photos table for each photo path in the product object
                            if (product.photos != null && product.photos.Any())
                            {
                                string insertProductPhotoQuery = @"
                                    INSERT INTO product_photos (product_id, photo_path)
                                    VALUES (@product_Id, @photo_Path);";

                                foreach (string photo_Path in product.photos)
                                {
                                    command.Parameters.Clear();
                                    command.CommandText = insertProductPhotoQuery;
                                    command.Parameters.AddWithValue("@product_Id", product_Id);
                                    command.Parameters.AddWithValue("@photo_Path", photo_Path);
                                    await command.ExecuteNonQueryAsync();
                                }
                            }

                            // Commit the transaction
                            transaction.Commit();

                            // Return the newly created product object with the product ID set
                            return new ProductDto
                            {
                                Product_Id = product_Id,
                                Product_Name = product.product_name,
                                Description = product.description,
                                Price = product.price,
                                Quantity = product.quantity,
                                CategoryIds = product.category_ids,
                                Photos = product.photos,
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        // Roll back the transaction if an exception occurs
                        transaction.Rollback();
                        throw ex;
                    }
                }

            }
        }


        public static async Task<ProductDto> UpdateProductAsync(int product_Id, ProductRequest product, string shopName)
        {
            // Create a new SqlConnection object
            using (SqlConnection connection = DatabaseUtility.ShopConnectionString(shopName))
            {
                // Open the database connection
                await connection.OpenAsync();

                // Begin a new transaction
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Update the record in the products table
                        string updateProductQuery = @"
                        UPDATE products
                        SET product_name = @product_name,
                            description = @description,
                            price = @price,
                            quantity = @quantity,
                            updated_at = GETDATE()
                        WHERE product_id = @product_id;";
                        using (SqlCommand command = new SqlCommand(updateProductQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@product_name", product.product_name);
                            command.Parameters.AddWithValue("@description", product.description);
                            command.Parameters.AddWithValue("@price", product.price);
                            command.Parameters.AddWithValue("@quantity", product.quantity);
                            command.Parameters.AddWithValue("@product_id", product_Id);

                            await command.ExecuteNonQueryAsync();

                            // Delete all records in the product_categories table for the given product ID
                            string deleteProductCategoriesQuery = @"
                            DELETE FROM product_categories
                            WHERE product_id = @product_id;";
                            command.CommandText = deleteProductCategoriesQuery;
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("product_id", product_Id);
                            await command.ExecuteNonQueryAsync();


                            // Insert records into the product_categories table for each category ID in the product object
                            if (product.category_ids != null && product.category_ids.Any())
                            {
                                string insertProductCategoryQuery = @"
                                    INSERT INTO product_categories (product_id, category_id)
                                    VALUES (@product_id, @category_id);";
                                command.CommandText = insertProductCategoryQuery;

                                foreach (int category_id in product.category_ids)
                                {
                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@product_Id", product_Id);
                                    command.Parameters.AddWithValue("@category_id", category_id);

                                    await command.ExecuteNonQueryAsync();
                                }
                            }
                            // Delete all records in the product_photos table for the given product ID
                            string deleteProductPhotosQuery = @"
                                DELETE FROM product_photos
                                WHERE product_id = @product_id;";
                            command.CommandText = deleteProductPhotosQuery;
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@product_Id", product_Id);
                            await command.ExecuteNonQueryAsync();


                            // Insert records into the product_photos table for each photo path in the product object
                            if (product.photos != null && product.photos.Any())
                            {
                                string insertProductPhotoQuery = @"
                                    INSERT INTO product_photos (product_id, photo_path)
                                    VALUES (@product_id, @photo_path);";
                                command.CommandText = insertProductPhotoQuery;

                                foreach (string photo_path in product.photos)
                                {
                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@product_Id", product_Id);
                                    command.Parameters.AddWithValue("@photo_path", photo_path);
                                    await command.ExecuteNonQueryAsync();
                                }
                            }
                        }

                        // Commit the transaction
                        transaction.Commit();

                        // Return the updated product object
                        return new ProductDto
                        {
                            Product_Id = product_Id,
                            Product_Name = product.product_name,
                            Description = product.description,
                            Price = product.price,
                            Quantity = product.quantity,
                            CategoryIds = product.category_ids,
                            Photos = product.photos
                        };
                    }
                    catch (Exception ex)
                    {
                        // Roll back the transaction if an exception occurs
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public static async Task<string> DeleteProductAsync(string shopName, int product_id)
        {
            using (SqlConnection connection = DatabaseUtility.ShopConnectionString(shopName))
            {
                await connection.OpenAsync();

                // Begin a new transaction
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Delete the product records from the product_categories table
                        string deleteProductCategoryQuery = @"
                        UPDATE products
                        SET Active = 0
                        WHERE product_id = @product_id;";
                        await connection.ExecuteAsync(deleteProductCategoryQuery, new { product_id }, transaction);

                        // Commit the transaction
                        transaction.Commit();

                        return "product removed successfully";
                    }
                    catch (Exception ex)
                    {
                        // Roll back the transaction if an exception occurs
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public static async Task<ProductDto> GetProductByIdAsync(string shopName, int product_id)
        {
            using (SqlConnection connection = DatabaseUtility.ShopConnectionString(shopName))
            {
                try
                {

                    string productQuery = @"
                    SELECT product_id, product_name, description,
                    price, quantity 
                    FROM products 
                    WHERE product_id = @id and active = 1";
                    var product = (await connection.QueryAsync<ProductDto>(productQuery, new { id = product_id })).FirstOrDefault();

                    if (product == null)
                    {
                        return null;
                    }

                    List<int>? valid_categories = new List<int>();
                    string categoryIdsQuery =
                        @"SELECT category_id
                            FROM product_categories
                            WHERE product_id = @product_id";
                    product.CategoryIds = (await connection.QueryAsync<int>(categoryIdsQuery, new { product_id })).ToList();
                    valid_categories = await DatabaseUtility.GetValidCategories(connection, product.CategoryIds);

                    string photosQuery = @"
                    SELECT Photo_Path
                    FROM product_photos
                    WHERE Product_Id = @id";
                    var photos = (await connection.QueryAsync<string>(photosQuery, new { id = product_id })).ToList();

                    var Discounts = await DatabaseUtility.calculateBestDiscount(connection, product.Product_Id, product.Price, valid_categories);

                    if (Discounts != null)
                    {
                        product.DiscountedPrice = Discounts.DiscountedPrice;
                        product.Discount = Discounts.HighestDiscount;
                    }
                    else
                    {
                        product.DiscountedPrice = product.Price;
                        product.Discount = null;
                    }

                    if (photos.Count > 0)
                    {
                        product.Photos = photos;
                    }


                    return product;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }


        public static async Task<ProductsResponse> GetProductsAsync(string shopName, List<int>? CategoryIds = null, string? name = "", string sortBy = "updated_at", string order = "ASC", int limit = 20, int page = 1)
        {
            try
            {
                using (SqlConnection connection = DatabaseUtility.ShopConnectionString(shopName))
                {
                    // Build the SQL query
                    var query = @"SELECT p.product_id AS Product_Id,
                             p.product_name AS Product_Name,
                             p.price AS Price,
                             p.quantity AS Quantity,
                             pp.photo_path AS Photo_path,
                             p.updated_at AS Updated_At,
                             ROW_NUMBER() OVER (PARTITION BY p.product_id ORDER BY pp.photo_id) AS [row_number]
                      FROM products p
                      LEFT JOIN product_photos pp ON p.product_id = pp.product_id";

                    // Add conditions based on the parameters
                    var parameters = new DynamicParameters();

                    if (CategoryIds != null && CategoryIds.Count() > 0)
                    {
                        query += " INNER JOIN product_categories pc ON p.product_id = pc.product_id";
                        query += " WHERE pc.category_id IN @CategoryIds";
                        parameters.Add("@CategoryIds", CategoryIds);
                    }

                    if (!string.IsNullOrEmpty(name))
                    {
                        if (query.Contains("WHERE"))
                            query += " AND p.product_name LIKE @ProductName";
                        else
                            query += " WHERE p.product_name LIKE @ProductName";

                        parameters.Add("@ProductName", "%" + name + "%");
                    }

                    if (query.Contains("WHERE"))
                        query += " AND p.Active = 1";
                    else
                        query += " WHERE p.Active = 1";

                    // Clone the query to retrieve the total count of rows
                    var countQuery = $"SELECT COUNT(*) FROM ({query}) AS TotalCount";

                    query =
$@"WITH cte_products AS ({query})
SELECT Product_Id, Product_Name, Price, Quantity, Photo_path, Updated_At
FROM cte_products
WHERE [row_number] = 1";


                    // Add sorting and paging
                    query += " ORDER BY " + sortBy + " " + order;
                    query += " OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";
                    parameters.Add("@Offset", (page - 1) * limit);
                    parameters.Add("@Limit", limit);
                    // Execute the queries
                    var products = await connection.QueryAsync<ProductsDto>(query, parameters);
                    var totalRows = await connection.ExecuteScalarAsync<int>(countQuery, parameters);

                    foreach (var product in products)
                    {
                        string productCategoryIdsQuery = 
                            @"SELECT category_id
					        FROM product_categories
					        WHERE product_id = @product_id";
                        var productCategoryIds = (await connection.QueryAsync<int>(productCategoryIdsQuery, new { product_id = product.Product_Id })).ToList();
                        var productValidCategoryIds = await DatabaseUtility.GetValidCategories(connection, productCategoryIds);

                        var Discounts = await DatabaseUtility.calculateBestDiscount(connection, product.Product_Id, product.Price, productValidCategoryIds);

                        if (Discounts != null)
                            product.DiscountedPrice = Discounts.DiscountedPrice;
                        else
                            product.DiscountedPrice = product.Price;
                    }

                    // Build the ProductsResponse object
                    var response = new ProductsResponse
                    {
                        Products = products,
                        Page = page,
                        Limit = limit,
                        TotalRows = totalRows
                    };

                    return response;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
