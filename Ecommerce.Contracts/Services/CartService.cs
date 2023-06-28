using Dapper;
using Ecommerce.Contracts.Models.Requests;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Utilities;
using System.Data.SqlClient;

namespace Ecommerce.Contracts.Services
{
    public class CartService
    {
        public static async Task<CartDto?> UpsertCartAsync(CartRequest request, string shop_name)
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
                {
                    await _dbConnection.OpenAsync();
                    using (SqlTransaction transaction = _dbConnection.BeginTransaction())
                    {

                        // Insert or update cart
                        string Query =
                                @"MERGE carts AS target
                        USING (VALUES (@user_id, GETDATE())) AS source (user_id, upserted_at)
                        ON target.user_id = source.user_id
                        WHEN MATCHED THEN
                            UPDATE SET upserted_at = source.upserted_at
                        WHEN NOT MATCHED THEN
                            INSERT (user_id, upserted_at)
                            VALUES (source.user_id, source.upserted_at);

                        SELECT cart_id FROM carts WHERE user_id = @user_id;";

                        var cartId = await _dbConnection.QueryFirstOrDefaultAsync<int>(Query, new { request.user_id }, transaction);

                        // Insert or update cart items
                        Query =
                            @"MERGE cart_items AS target
                        USING (VALUES (@cart_id, @product_id, @quantity)) AS source (cart_id, product_id, quantity)
                        ON target.cart_id = source.cart_id AND target.product_id = source.product_id
                        WHEN MATCHED THEN
                            UPDATE SET quantity = source.quantity
                        WHEN NOT MATCHED THEN
                            INSERT (cart_id, product_id, quantity)
                            VALUES (source.cart_id, source.product_id, source.quantity);";

                        foreach (var item in request.cart_items)
                        {
                            await _dbConnection.ExecuteAsync(Query, new { cart_id = cartId, item.product_id, item.quantity }, transaction);
                        }

                        transaction.Commit();

                        return await GetCartAsync(request.user_id, shop_name);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public static async Task<CartDto?> GetCartAsync(string user_id, string shop_name)
        {
            try
            {
                var cart = new CartDto()
                {
                    CartItems = new List<CartItemDto>()
                };
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
                {
                    await _dbConnection.OpenAsync();

                    string query = @"
                        SELECT [cart_id]
                              ,[user_id]
                              ,[upserted_at]
                          FROM [carts]
                          WHERE user_id = @user_id";

                    cart = await _dbConnection.QueryFirstOrDefaultAsync<CartDto>(query, new { user_id });

                    if (cart != null)
                    {
                        query = @"SELECT [cart_items].[cart_item_id]
                                      ,[cart_items].[cart_id]
                                      ,[cart_items].[product_id]
                                      ,[products].[product_name]
                                      ,[cart_items].[quantity]
                                  FROM [cart_items]
                                  JOIN [products] ON [cart_items].[product_id] = [products].[product_id]
                                  WHERE cart_id = @cart_id";

                        cart.CartItems = await _dbConnection.QueryAsync<CartItemDto>(query, new { cart.Cart_Id });

                        query = "Select Price from Products where product_id = @product_id";
                        foreach (var item in cart.CartItems)
                        {
                            var price = (await _dbConnection.QueryAsync<int>(query, new { product_id = item.Product_Id })).FirstOrDefault();
                            item.Price = price;
                            item.DiscountedPrice = (await DatabaseUtility.calculateBestDiscount(_dbConnection, item.Product_Id, price)).DiscountedPrice;
                            cart.TotalPrice += item.DiscountedPrice * item.Quantity;
                        }
                    }
                    return cart;
                }
            }
            catch
            {
                throw;
            }
        }
        public static async Task<CartDto?> RemoveProductFromCartAsync(string shop_name, string userId, int productId)
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
                {
                    await _dbConnection.OpenAsync();

                    using (SqlTransaction transaction = _dbConnection.BeginTransaction())
                    {
                        // Get cart ID for the user
                        string cartQuery = "SELECT cart_id FROM carts WHERE user_id = @UserId";
                        int cartId = await _dbConnection.ExecuteScalarAsync<int>(cartQuery, new { UserId = userId }, transaction);

                        // Check if the product is already in the user's cart
                        string checkCartItemQuery = "SELECT cart_item_id, quantity FROM cart_items WHERE cart_id = @CartId AND product_id = @ProductId";
                        CartItemDto cartItem = await _dbConnection.QueryFirstOrDefaultAsync<CartItemDto>(checkCartItemQuery, new { CartId = cartId, ProductId = productId }, transaction);
                        if (cartItem == null)
                            return null;

                        // Product is in the cart, remove it
                        string removeCartItemQuery = "DELETE FROM cart_items WHERE cart_item_id = @CartItemID";
                        await _dbConnection.ExecuteAsync(removeCartItemQuery, new { CartItemID = cartItem.Cart_Item_Id }, transaction);

                        // Update the cart's upserted_at timestamp
                        string updateCartTimestampQuery = "UPDATE carts SET upserted_at = GETDATE() WHERE cart_id = @CartId";
                        await _dbConnection.ExecuteAsync(updateCartTimestampQuery, new { CartId = cartId }, transaction);

                        transaction.Commit();

                        // Return updated response
                        return await GetCartAsync(userId, shop_name);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public static async Task<CartDto?> ClearUserCartAsync(string shop_name, string userId)
        {
            try
            {
                using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
                {
                    await _dbConnection.OpenAsync();

                    using (SqlTransaction transaction = _dbConnection.BeginTransaction())
                    {
                        // Get cart ID for the user
                        string cartQuery = "SELECT cart_id FROM carts WHERE user_id = @UserId";
                        int? cartId = await _dbConnection.ExecuteScalarAsync<int?>(cartQuery, new { UserId = userId }, transaction);

                        // If the user doesn't have a cart, return null
                        if (cartId == null)
                            return null;

                        // Remove all items from the cart
                        string removeCartItemsQuery = "DELETE FROM cart_items WHERE cart_id = @CartId";
                        await _dbConnection.ExecuteAsync(removeCartItemsQuery, new { CartId = cartId }, transaction);

                        // Update the cart's upserted_at timestamp
                        string updateCartTimestampQuery = "UPDATE carts SET upserted_at = GETDATE() WHERE cart_id = @CartId";
                        await _dbConnection.ExecuteAsync(updateCartTimestampQuery, new { CartId = cartId }, transaction);

                        transaction.Commit();

                        // Return response
                        return await GetCartAsync(userId, shop_name);
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}