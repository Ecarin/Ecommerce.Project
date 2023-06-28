using Dapper;
using Ecommerce.Contracts.Models.Requests;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Utilities;
using System.Data.SqlClient;

namespace Ecommerce.Contracts.Services
{
    public class OrderService
    {
        public static async Task<OrderResponse> CreateOrderAsync(string shop_name, CreateOrderRequest request)
        {
            //get connection string
            using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
            {
                await _dbConnection.OpenAsync();

                OrderResponse response = new OrderResponse();
                response.Order = new OrderDto();
                response.Order.Order_Items = new List<Order_ItemDto>();

                //create a transaction
                using (SqlTransaction transaction = _dbConnection.BeginTransaction())
                {
                    try
                    {
                        var cartDto = await CartService.GetCartAsync(request.User_Id, shop_name);
                        if (cartDto.CartItems != null && cartDto.CartItems.Count() > 0)
                        {
                            //insert an order
                            string insertOrderQuery =
                                @"INSERT INTO [dbo].[orders]
                                       ([user_id]
                                       ,[user_address_id]
                                       ,[full_address]
                                       ,[shipping_cost]
                                       ,[total_price]
                                       ,[order_description]
                                       ,[receipt_photo]
                                       ,[user_full_name])
                                 VALUES
                                       (@user_id
                                       ,@user_address_id
                                       ,@full_address
                                       ,@shipping_cost
                                       ,@total_price
                                       ,@order_description
                                       ,@receipt_photo
                                       ,@user_full_name)
                                SELECT SCOPE_IDENTITY();";

                            //get user full address
                            string userFull_AddressQuery =
                                "select (country + ' ' + state + ' ' + city + ' ' + street + ' ' + zipcode)" +
                                " as full_address  from user_address where address_id = @address_id";
                            response.Order.Full_Address = await _dbConnection.ExecuteScalarAsync<string>(userFull_AddressQuery, new { address_id = request.User_Address_Id }, transaction);


                            string userFull_NameQuery = "SELECT COALESCE(CONCAT_WS(' ', name, last_name), '') AS user_full_name FROM users where user_id = @User_Id";
                            response.Order.User_Full_Name = await _dbConnection.ExecuteScalarAsync<string>(userFull_NameQuery, new { request.User_Id }, transaction);

                            //create order and fetch Order_ID
                            int createdOrderId = await _dbConnection.ExecuteScalarAsync<int>(insertOrderQuery,
                                new
                                {
                                    user_id = request.User_Id,
                                    user_address_id = request.User_Address_Id,
                                    full_address = response.Order.Full_Address,
                                    shipping_cost = request.Shipping_Cost,
                                    total_price = cartDto.TotalPrice + request.Shipping_Cost,
                                    order_description = request.Order_Description,
                                    receipt_photo = request.Receipt_Photo_Path,
                                    user_full_name = response.Order.User_Full_Name
                                }, transaction);


                            response.Order.Order_Id = createdOrderId;
                            response.Order.User_Id = request.User_Id;
                            response.Order.User_Address_Id = request.User_Address_Id;
                            response.Order.shipping_cost = request.Shipping_Cost;
                            response.Order.Total_Price = cartDto.TotalPrice + request.Shipping_Cost;
                            response.Order.Order_Description = request.Order_Description;
                            response.Order.Receipt_Photo = request.Receipt_Photo_Path;
                            response.Order.Order_Status = "Pending";
                            response.Order.Order_Date = DateTime.Now;

                            //now add order items

                            string insertOrderItemsQuery =
                                @"INSERT INTO [dbo].[order_items]
                                       ([order_id]
                                       ,[product_id]
                                       ,[product_name]
                                       ,[tag_price]
                                       ,[final_price]
                                       ,[quantity])
                                 VALUES
                                       (@order_id
                                       ,@product_id
                                       ,@product_name
                                       ,@tag_price
                                       ,@final_price
                                       ,@quantity)
                                SELECT SCOPE_IDENTITY();";
                            string updateProductQuantity =
                                @"Update [dbo].[products]
                                    Set Quantity = Quantity - @quantity
                                Where product_id = @product_id";


                            foreach (var item in cartDto.CartItems)
                            {

                                //create order item and get the created OrderItemID
                                int createdOrderItemId = await _dbConnection.ExecuteScalarAsync<int>(insertOrderItemsQuery,
                                    new
                                    {
                                        order_id = createdOrderId,
                                        product_id = item.Product_Id,
                                        product_name = item.Product_Name,
                                        tag_price = item.Price,
                                        final_price = item.DiscountedPrice,
                                        quantity = item.Quantity,
                                    }, transaction);


                                try
                                {
                                    await _dbConnection.QueryAsync(updateProductQuantity,
                                        new
                                        {
                                            quantity = item.Quantity,
                                            product_id = item.Product_Id
                                        }, transaction);
                                }
                                catch (Exception err)
                                {
                                    response.Order = null;
                                    response.Success = false;
                                    response.Message = $"Quantity of {item.Product_Id}:{item.Product_Name} is not enough!";
                                    return response;
                                }

                                // remove product drom cart
                                await CartService.RemoveProductFromCartAsync(shop_name, request.User_Id, item.Product_Id);
                                response.Order.Order_Items.Add(
                                    new Order_ItemDto()
                                    {
                                        Order_Id = createdOrderId,
                                        Order_Item_Id = createdOrderItemId,
                                        Product_Id = item.Product_Id,
                                        Product_Name = item.Product_Name,
                                        Tag_Price = item.Price,
                                        Final_Price = item.DiscountedPrice,
                                        Quantity = item.Quantity,
                                    });


                            }


                            //insert receipt photo to database
                            string insertReceiptPhotoQuery =
                                @"INSERT INTO [dbo].[receipt_photos]
                                       ([order_id]
                                       ,[photo_path])
                                 VALUES
                                       (@order_id
                                       ,@photo_path)";


                            await _dbConnection.QueryAsync(insertReceiptPhotoQuery,
                                new
                                {
                                    order_id = createdOrderId,
                                    photo_path = request.Receipt_Photo_Path
                                }, transaction);
                            transaction.Commit();
                            response.Success = true;
                            response.Message = "Order created succesfully";
                            return response;


                        }
                        else
                        {
                            response.Order = null;
                            response.Success = false;
                            response.Message = "User cart is empty";
                            transaction.Rollback();
                            return response;
                        }

                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction and throw exception
                        response.Order = null;
                        response.Success = false;
                        response.Message = ex.Message;
                        transaction.Rollback();
                        return response;
                    }
                }
            }
        }
        public static async Task<OrderResponse> GetOrderByIdAsync(string shop_name, int orderId)
        {
            using (var connection = DatabaseUtility.ShopConnectionString(shop_name))
            {
                await connection.OpenAsync();

                // Retrieve the order by its ID
                var orderQuery = @"
                    SELECT 
                        o.Order_Id,
                        o.User_Id,
                        o.User_Address_Id,
                        o.user_full_name,
                        o.full_address,
                        o.shipping_cost,
                        o.Total_Price,
                        o.Order_Description,
                        o.Receipt_Photo,
                        o.Order_Status,
                        o.Tracking_Code,
                        o.Order_Date,
                        o.Updated_At,
                        oi.Order_Item_Id,
                        oi.Order_Id,
                        oi.Product_Id,
                        oi.Product_Name,
                        oi.Tag_Price,
                        oi.Final_Price,
                        oi.Quantity
                    FROM Orders o
                    INNER JOIN Order_Items oi ON o.Order_Id = oi.Order_Id
                    WHERE o.Order_Id = @orderId";

                var orderDictionary = new Dictionary<int, OrderDto>();
                var orderItemList = new List<Order_ItemDto>();

                (await connection.QueryAsync<OrderDto, Order_ItemDto, OrderDto>(
                    orderQuery,
                    (order, orderItem) =>
                    {
                        if (!orderDictionary.TryGetValue(order.Order_Id, out OrderDto orderEntry))
                        {
                            orderEntry = order;
                            orderEntry.Order_Items = new List<Order_ItemDto>();
                            orderDictionary.Add(orderEntry.Order_Id, orderEntry);
                        }

                        orderEntry.Order_Items.Add(orderItem);
                        return orderEntry;
                    },
                    new { orderId },
                    splitOn: "Order_Item_Id")
                ).FirstOrDefault();

                // If no order was found with the provided ID, return an error response
                if (orderDictionary.Count == 0)
                {
                    return new OrderResponse { Success = false, Message = "Order not found." };
                }

                // Otherwise, return the order
                var order = orderDictionary.Values.FirstOrDefault();
                return new OrderResponse { Success = true, Order = order };
            }
        }


        public static async Task<OrdersResponse> GetOrdersByUserIdAsync(string shop_name, string user_id, string sortBy = "order_date", string order = "asc", string? order_status = null, int limit = 20, int page = 1)
        {
            using (var connection = DatabaseUtility.ShopConnectionString(shop_name))
            {
                await connection.OpenAsync();

                // Retrieve the order by its ID
                var orderQuery = $@"
                    SELECT 
                        o.Order_Id,
                        o.User_Id,
                        o.User_Address_Id,
                        o.User_full_name,
                        o.full_address,
                        o.shipping_cost,
                        o.Total_Price,
                        o.Order_Description,
                        o.Receipt_Photo,
                        o.Order_Status,
                        o.Tracking_Code,
                        o.Order_Date,
                        o.Updated_At,
                        oi.Order_Item_Id,
                        oi.Order_Id,
                        oi.Product_Id,
                        oi.Product_Name,
                        oi.Tag_Price,
                        oi.Final_Price,
                        oi.Quantity
                    FROM Orders o
                    INNER JOIN Order_Items oi ON o.Order_Id = oi.Order_Id
                    WHERE o.user_id = @user_id
                    AND (@order_status IS NULL OR o.order_status = @order_status)
                    ORDER BY {sortBy} {order}
                    OFFSET @Offset ROWS FETCH NEXT @limit ROWS ONLY;";

                string queryTotalCount = @"SELECT COUNT(*) FROM Orders o
                    INNER JOIN Order_Items oi ON o.Order_Id = oi.Order_Id
                    WHERE o.user_id = @user_id
                    AND (@order_status IS NULL OR o.order_status = @order_status)";

                int totalRows = await connection.ExecuteScalarAsync<int>(queryTotalCount, new
                {
                    user_id,
                    order_status,
                });

                var orderDictionary = new Dictionary<int, OrderDto>();
                var orderItemList = new List<Order_ItemDto>();

                (await connection.QueryAsync<OrderDto, Order_ItemDto, OrderDto>(
                    orderQuery,
                    (order, orderItem) =>
                    {
                        if (!orderDictionary.TryGetValue(order.Order_Id, out OrderDto orderEntry))
                        {
                            orderEntry = order;
                            orderEntry.Order_Items = new List<Order_ItemDto>();
                            orderDictionary.Add(orderEntry.Order_Id, orderEntry);
                        }

                        orderEntry.Order_Items.Add(orderItem);
                        return orderEntry;
                    },
                    new { user_id, order_status , limit, page,
                        offset = (page - 1) * limit
                    },
                    splitOn: "Order_Item_Id")
                ).FirstOrDefault();

                // If no order was found with the provided ID, return an error response
                if (orderDictionary.Count == 0)
                {
                    return new OrdersResponse()
                    {
                        Orders = null,
                        TotalRows = totalRows,
                        Page = page,
                        Limit = limit,
                    };
                }

                // Otherwise, return the order
                var orderResult = orderDictionary.Values;
                var response = new OrdersResponse()
                {
                    Orders = orderResult,
                    TotalRows = totalRows,
                    Page = page,
                    Limit = limit,
                };
                return response;
            }
        }
        public static async Task<OrdersResponse> GetOrdersAsync(string shop_name, int? orderId = null, string? userFullName = "", string sortBy = "order_date", string order = "asc", int? totalPrice = null, string? orderStatus = null, int limit = 20, int page = 1)
        {
            try
            {
                using var connection = DatabaseUtility.ShopConnectionString(shop_name);
                var query = $@"SELECT * FROM orders
                  WHERE (@orderId IS NULL OR order_id = @orderId)
                    AND (@userFullName IS NULL OR user_full_name LIKE CONCAT('%', @userFullName, '%'))
                    AND (@totalPrice IS NULL OR total_price = @totalPrice)
                    AND (@orderStatus IS NULL OR order_status = @orderStatus)
                    ORDER BY {sortBy} {order}
                    OFFSET @Offset ROWS FETCH NEXT @limit ROWS ONLY;";

                string queryTotalCount = @"SELECT COUNT(*) From orders
                WHERE (@orderId IS NULL OR order_id = @orderId)
                    AND (@userFullName IS NULL OR user_full_name LIKE CONCAT('%', @userFullName, '%'))
                    AND (@totalPrice IS NULL OR total_price = @totalPrice)
                    AND (@orderStatus IS NULL OR order_status = @orderStatus);";

                var orders = await connection.QueryAsync<OrderDto>(query, new
                {
                    orderId,
                    userFullName,
                    totalPrice,
                    orderStatus,
                    limit,
                    offset = (page - 1) * limit
                });
                int totalRows = await connection.ExecuteScalarAsync<int>(queryTotalCount, new
                {
                    orderId,
                    userFullName,
                    totalPrice,
                    orderStatus,
                });

                var response = new OrdersResponse()
                {
                    Orders = orders,
                    TotalRows = totalRows,
                    Page = page,
                    Limit = limit,
                };
                return response;

            }
            catch (Exception ex)
            {
                return new OrdersResponse()
                {
                    TotalRows = 0,
                    Page = 0,
                    Limit = 0,
                };
            }
        }

        //public static async Task<OrderResponse> UpdateOrderAsync(string shopName, UpdateOrderRequest request, int orderId)
        //{
        //    using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shopName))
        //    {
        //        await _dbConnection.OpenAsync();
        //        using (var transaction = _dbConnection.BeginTransaction())
        //        {
        //            try
        //            {
        //                string userFull_Address = await _dbConnection.ExecuteScalarAsync<string>(
        //                    "select user_address_id from [dbo].[orders] where order_id = @orderId"
        //                    , new { orderId });

        //                if (request.User_Address_Id != null)
        //                {
        //                    string userFull_AddressQuery =
        //                     "select (country + ' ' + state + ' ' + city + ' ' + street + ' ' + zipcode)" +
        //                     " as full_address  from user_address where address_id = @address_id";
        //                    userFull_Address = await _dbConnection.ExecuteScalarAsync<string>(userFull_AddressQuery, new { address_id = request.User_Address_Id }, transaction);
        //                }

        //                string updateOrderQuery =
        //                    @"UPDATE orders SET 
        //                        user_address_id = COALESCE(@userAddressId, user_address_id),
        //                        full_address = COALESCE(@full_address, full_address),
        //                        receipt_photo = COALESCE(@receiptPhoto, receipt_photo),
        //                        updated_at = GETDATE()
        //                    WHERE order_id = @orderId;";

        //                await _dbConnection.QueryAsync(updateOrderQuery,
        //                    new
        //                    {
        //                        userAddressId = request.User_Address_Id,
        //                        full_address = userFull_Address,
        //                        receipt_photo = request.Receipt_Photo_Path,
        //                    }
        //                    , transaction);

        //                return new OrderResponse()
        //                {
        //                    Order = (await GetOrderByIdAsync(shopName, orderId)).Order,
        //                    Message = "Order updated successfuly",
        //                    Success = true,
        //                };
        //            }
        //            catch (Exception ex)
        //            {
        //                return new OrderResponse()
        //                {
        //                    Message = ex.Message,
        //                    Success = false,
        //                };
        //            }
        //        }
        //    }
        //}
        public static async Task<OrderResponse> UpdateOrderStatusAsync(string shop_name, UpdateOrderRequest request, int orderId)
        {
            using (SqlConnection _dbConnection = DatabaseUtility.ShopConnectionString(shop_name))
            {
                await _dbConnection.OpenAsync();
                using (var transaction = _dbConnection.BeginTransaction())
                {
                    try
                    {
                        int existance = await _dbConnection.ExecuteScalarAsync<int>("select count(*) from orders where order_id = @orderId and order_status like '%Cancelled%'", new {orderId}, transaction);
                        if (existance > 0)
                        {
                            return new OrderResponse()
                            {
                                Message = "You cannot modify cancelled order!",
                                Success = false
                            };
                        }
                        string Query =
                            @"Update [dbo].[orders] Set order_status = @order_status, updated_at = GETDATE()";
                        string recoverProducts =
                            @"UPDATE p
                                SET p.quantity = p.quantity + oi.quantity
                                FROM products p
                                JOIN order_items oi ON oi.product_id = p.product_id
                                JOIN orders o ON o.order_id = oi.order_id
                                WHERE o.order_status LIKE '%Cancelled%'";

                        switch (request.Order_Status)
                        {
                            case "Pending":
                            case "Processing":
                            case "Packing":
                            case "CancelledByCustomer":
                            case "CancelledDueToUnavailability":
                            case "CancelledByAdmin":
                                Query += ", tracking_code = NULL ";
                                break;
                            case "Shipped":
                                Query += ", tracking_code = @tracking_code ";
                                break;
                            default:
                                break;
                        }
                        Query += "Where order_id = @orderId";
                        await _dbConnection.QueryAsync(Query, new
                        {
                            orderId = orderId,
                            order_status = request.Order_Status.ToString(),
                            tracking_code = request.Tracking_Code,
                        }, transaction);
                        await _dbConnection.QueryAsync(recoverProducts, new {},transaction);
                        transaction.Commit();
                        return new OrderResponse()
                        {
                            Message = "Order updated successfully",
                            Success = true,
                            Order = (await GetOrderByIdAsync(shop_name, orderId)).Order
                        };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new OrderResponse()
                        {
                            Message = ex.Message,
                            Success = false
                        };
                    }
                }
            }
        }
    }
}
