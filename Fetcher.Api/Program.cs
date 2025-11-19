using Fetcher.Api.Features.Posts;
using Fetcher.Api.Infrastructure.Configs;
using Fetcher.Api.Infrastructure.External;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.Configure<ExternalApiConfig>(builder.Configuration.GetSection("ExternalPostApi"));
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddHttpClient<IPostApiClient, PostApiClient>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
