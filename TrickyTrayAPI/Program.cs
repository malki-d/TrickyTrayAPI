using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using TrickyTrayAPI.Repositories;
using TrickyTrayAPI.Services;
using WebApi.Data;

// Add services to the container.

Log.Information("Starting Store API application");
var builder = WebApplication.CreateBuilder(args);

// Add Serilog configuration
Log.Logger = new LoggerConfiguration()
.WriteTo.Console()
.WriteTo.File("logs/student-api. log", rollingInterval: RollingInterval.Day)
.CreateLogger();

// Add Serilog
builder.Host.UseSerilog();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer("Server=localhost,1433;Database=DemoToTest;User Id=SA;Password=Str0ng@Passw0rd123;TrustServerCertificate=True;"));
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DemoToTest;Trusted_Connection=True;TrustServerCertificate=True;"));
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer("Server=.\\SQLEXPRESS;Database=DemoToTest;Trusted_Connection=True;TrustServerCertificate=True;"));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=DESKTOP-NRV805A;Database=DemoToTest_1;Trusted_Connection=True;TrustServerCertificate=True;"));
builder.Services.AddScoped<IDonorService, DonorService>();
builder.Services.AddScoped<IDonorRepository, DonorRepository>();

builder.Services.AddScoped<IGiftService, GiftService>();
builder.Services.AddScoped<IGiftRepository, GiftRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
