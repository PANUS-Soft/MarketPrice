using MarketPrice.Data;
using Microsoft.EntityFrameworkCore;
using EFCore.CheckConstraints;
using MarketPrice.Domain;
using MarketPrice.Services.Interfaces;
using MarketPrice.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<MarketPriceDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString(AppConstants.DatabaseConnectionString));
    options.UseEnumCheckConstraints();
});

//builder.Services.AddDbContextFactory<MarketPriceDbContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString(AppConstants.DatabaseConnectionString));
//    options.UseEnumCheckConstraints();

//});
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
