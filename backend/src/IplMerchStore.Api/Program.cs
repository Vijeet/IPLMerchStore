using IplMerchStore.Api.Middleware;
using IplMerchStore.Application.Interfaces;
using IplMerchStore.Infrastructure.Persistence;
using IplMerchStore.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "IPL Merch Store API",
        Version = "v1",
        Description = "Backend API for IPL merchandise ecommerce platform"
    });
});

// Add database context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.MigrationsAssembly("IplMerchStore.Infrastructure");
    });
});

// Add application services
builder.Services.AddScoped<IFranchiseService, FranchiseService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Add health checks
builder.Services.AddHealthChecks();

// Add logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Middleware pipeline
app.UseRouting();
app.UseCors("AllowAll");

// Add global exception handling middleware
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Use health checks
app.MapHealthChecks("/health");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "IPL Merch Store API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class Program { }
