using Fetcher.Api.Common.Middlewares;
using Fetcher.Api.Features.Posts;
using Fetcher.Api.Infrastructure.Configs;
using Fetcher.Api.Infrastructure.External;
using Fetcher.CacheService.Cache;
using Fetcher.CacheService.Configs;
using Fetcher.CacheService.Data;
using Fetcher.CacheService.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.Configure<ExternalApiConfig>(builder.Configuration.GetSection("ExternalPostApi"));
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddHttpClient<IPostApiClient, PostApiClient>();

builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddSingleton<ICacheStoreService, CacheStoreService>();
builder.Services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
builder.Services.AddHostedService<CacheCleanupService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
