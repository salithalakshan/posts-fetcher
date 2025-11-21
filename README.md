# Posts-Fetcher

## 3rd Party API

- Base URL: `https://jsonplaceholder.typicode.com/`
- No API key is required

##  Libraries Used
- ASP.NET 9                                             - Web API framework.
- Microsoft.Data.SqlClient                              - Persist data in SQL Server with ADO.NET
- Microsoft.Extensions.Hosting.Abstractions             - To Run background task fro Cache Cleanup
- Microsoft.Extensions.Options.ConfigurationExtensions  - Configuration map to concrete C# classes

## Build & Run Instructions
- Run the provided script in the SQL Server
- Set connection string properly in the appsettings.json file
- Set Fetcher.Api as startup project
- Run the application
- Use Fetcher.Api.http file to check API endpoint 