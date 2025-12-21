using Microsoft.EntityFrameworkCore;
using System;
using WebApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
    options.UseSqlServer("Server=DESKTOP-1VUANBN;Database=DemoToTest;Trusted_Connection=True;TrustServerCertificate=True;"));
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
