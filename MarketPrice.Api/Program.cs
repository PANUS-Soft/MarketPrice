using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MarketPrice.Data;
using Microsoft.EntityFrameworkCore;
using EFCore.CheckConstraints;
using MarketPrice.Services.Interfaces;
using MarketPrice.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICE REGISTRATION ---
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();

// --- 2. CONFIGURE ASYMMETRIC AUTHENTICATION ---
// Get Public Key from appsettings.json
var publicKeyXml = builder.Configuration["Authentication:PublicKey"];
var rsa = RSA.Create();
rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKeyXml), out _);
var publicKey = new RsaSecurityKey(rsa);

// This creates the "SigningCredentials" using the PRIVATE KEY from Secret Manager
// (The Secret Manager automatically overrides appsettings during development)
var privateKeyXml = builder.Configuration["Authentication:PrivateKey"];
var rsaPrivate = RSA.Create();
rsaPrivate.ImportFromPem(privateKeyXml); // Ensure your secret is in PEM format
var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsaPrivate), SecurityAlgorithms.RsaSha256);

// Inject credentials so TokenService can sign tokens
builder.Services.AddSingleton(signingCredentials);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = publicKey // <-- PUBLIC KEY TO VERIFY TOKEN
        };
    });

builder.Services.AddControllers();
builder.Services.AddDbContextFactory<MarketPriceDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MarketPrice"));
    options.UseEnumCheckConstraints();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- 3. ADD AUTHENTICATION TO PIPELINE ---
app.UseAuthentication(); // Who are you?
app.UseAuthorization();  // Are you allowed?

app.MapControllers();

app.Run();