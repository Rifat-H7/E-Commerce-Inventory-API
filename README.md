# E-Commerce Inventory API - Setup Instructions
This guide will help you set up and run the E-Commerce Inventory API locally on your development machine.
Prerequisites
Before running the project, make sure you have:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) 
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (included with Visual Studio) or SQL Server Express  
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or [VS Code](https://code.visualstudio.com/) with the C# extension  
- [Git](https://git-scm.com/) 
## Getting Started
### 1. Clone the Repository
git clone https://github.com/Rifat-H7/E-Commerce-Inventory-API.git
cd ecommerce-inventory-api
### 2. Restore NuGet Packages
Navigate to the solution root and restore all dependencies:

    dotnet restore
### 3. Database Configuration
#### Option A: SQL Server (Recommended for Development)
The default connection string uses SQL Server. No additional setup required if you have LocalDB installed.
Default Connection String:

`"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ECommerceInventoryDB;Trusted_Connection=true;MultipleActiveResultSets=true"`

#### Option B: SQL Server Express or Full SQL 

If you're using SQL Server Express or a full SQL Server instance, update the connection string in appsettings.json:

`{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ECommerceInventoryDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}`

For SQL Server with authentication:

`{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ECommerceInventoryDB;User Id=your-username;Password=your-password;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}`
### 4. JWT Configuration
Update the JWT settings in appsettings.json with your own secret key:
{
  "Jwt": {
    "Secret": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "ECommerceAPI",
    "Audience": "ECommerceAPIUsers",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  }
}
Important: Generate a strong secret key for production use. You can use online tools or PowerShell:
## Generate a random 64-character key
[System.Web.Security.Membership]::GeneratePassword(64, 0)
### 5. Database Setup
Using Entity Framework Migrations (Recommended)
1.	Install the EF Core tools globally (if not already installed):
   
        dotnet tool install --global dotnet-ef
3.	Navigate to the API project:
cd ECommerceAPI.API
4.	Create the initial migration:
   
        dotnet ef migrations add InitialCreate
6.	Update the database:
   
        dotnet ef database update
Alternative: Automatic Database Creation
The application is configured to automatically create the database on first run using context.Database.EnsureCreated() in Program.cs. Simply run the application and the database will be created automatically.
### 6. Build the Solution
        dotnet build
### 7. Run the Application
Navigate to the API project and run:
cd ECommerceAPI.API
        dotnet run
Or run from the solution root:
        dotnet run --project ECommerceAPI.API
The API will be available at:

•	HTTP: http://localhost:5108

•	HTTPS: https://localhost:7160

•	Swagger UI: https://localhost:7160 (redirects to Swagger in development)
