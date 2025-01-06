# Ecommerce.Project

Welcome to the `Ecommerce.Project` repository! This project was my first attempt at building a functional e-commerce platform using **.NET 7** and **C# 11**. While this was an exciting learning journey, this repository is now **archived** and **no longer actively maintained**.

---

## 🖥️ Frontend Project

This API is complemented by a frontend project built by my friend. You can find it here:
[Telegram Web App Shop](https://github.com/mojtaba1180/telegram-web-app-shop)

The frontend integrates with this API to provide a seamless shopping experience. Feel free to check it out and experiment with both projects together.

---

## 🛠️ Project Overview

This project is an API-driven e-commerce platform featuring:
- **Core Features**: 
  - Product and Category management
  - User accounts and Address handling
  - Cart and Order processing
  - Discount and Promotions
- **Architecture**:
  - Modular separation with `Ecommerce.API` and `Ecommerce.Contracts`.
  - DTO-based request and response handling.
- **Database Interaction**:
  - Lightweight ORM using **Dapper** for database queries.
- **Documentation**:
  - OpenAPI documentation powered by **Swashbuckle.AspNetCore**.

---

## 📂 Project Structure

### **Ecommerce.API**
- **Controllers**:  
  Handles RESTful API endpoints for resources like:
  - Products: `ProductsController.cs`
  - Users: `UsersController.cs`
  - Orders: `OrdersController.cs`
- **Properties**:  
  Contains debug configurations in `launchSettings.json`.

### **Ecommerce.Contracts**
- **Models**:
  - **Tables**: Defines database tables as DTOs, such as `ProductDto`, `OrderDto`, and `UserDto`.
  - **Requests**: Maps API request payloads (e.g., `ProductRequest`, `UserRequest`).
  - **Utilities**: Includes helper classes for responses (`ResponseDto`), database operations, and discounts.
- **Services**:  
  Implements business logic for features like:
  - Product Management: `ProductService.cs`
  - User Authentication: `UserService.cs`
  - Cart Handling: `CartService.cs`
  - Order Processing: `OrderService.cs`

---

## ⚠️ Known Issues and Limitations

- **Environment Configuration**:  
  `.env` files are missing. Configuration was handled using hard-coded text files that are included in the repository. While these files are non-functional now (tokens expired, servers down), this approach is **not secure** and **not recommended**.
  
- **Legacy Practices**:  
  This project reflects my early development practices and lacks proper abstractions, clean architecture, and robust error handling.

- **Security Concerns**:  
  Sensitive data was stored in plaintext and is no longer valid. However, this highlights the need for secure environment management.

---

## 🚀 Future Plans

I plan to rewrite this project from scratch with a focus on:
- **Modern Best Practices**: Adopting clean architecture, SOLID principles, and dependency injection.
- **Security**: Using environment variables and secure secrets management.
- **Improved Features**: Enhancing functionality and adding better scalability.

When the new repository is ready, I’ll link it here.

---

## 📝 How to Use

Although this repository is archived, you can explore its features by following these steps:

1. **Clone the repository**:
   ```bash
   git clone https://github.com/Ecarin/ecommerce.project.git
   cd ecommerce.project
   ```
2. **Open the project**:
   - Open the project in your favorite IDE or text editor. Recommended IDEs for this project include:
     - **Visual Studio 2022**: Fully supports .NET 7 and offers great debugging tools.
     - **JetBrains Rider**: A fast and powerful cross-platform IDE.
   - Ensure that all dependencies are restored. If not, run:
     ```bash
     dotnet restore
     ```

3. **Update the configuration**:
   - This project does not have a proper `.env` file. Instead, I used text files and hard-coded values to store configurations (😅). 
   - Look for files in the repository that might serve as environment configurations. They are scattered around and include sensitive information (all of which are expired and non-functional now).  
   - Update these configurations with your own valid API keys, database connection strings, etc.

4. **Run the project**:
   - Build and run the application using the following command:
     ```bash
     dotnet run --project Ecommerce.API
     ```
   - Alternatively, you can run the application from your IDE by setting `Ecommerce.API` as the startup project.

5. **Access the API documentation**:
   - After starting the application, Swagger UI will be available at:
     ```
     http://localhost:<port>/swagger
     ```
   - Use this interface to explore and test the available endpoints.

6. **Testing the API**:
   - The API can be fully tested using the **Swagger UI**, available at:
     ```
     http://localhost:<port>/swagger
     ```
   - Swagger provides an interactive interface to explore, test, and understand all available API endpoints.
   - Optionally, you can use external tools like **Postman** or **curl** for testing specific scenarios or integrating with other systems. For example:
     ```bash
     curl -X GET http://localhost:<port>/api/products
     ```


---

## 📞 Contact

If you have any questions, suggestions, or feedback about this project, feel free to reach out to me:

- **Telegram**: [@ecarin](https://t.me/ecarin)  
- **GitHub Profile**: [My GitHub](https://github.com/Ecarin)  
