using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MarketPrice.Data;
using Microsoft.EntityFrameworkCore;
using MarketPrice.Services.Interfaces;
using MarketPrice.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// --- 1. DATABASE REGISTRATION ---
var connectionString = builder.Configuration.GetConnectionString("MarketPrice");
builder.Services.AddDbContext<MarketPriceDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- 2. SERVICE REGISTRATION ---
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();

// --- 3. CONFIGURE ASYMMETRIC AUTHENTICATION ---
// Note: Updated to match your new appsettings key "PUBLIC_KEY"
var publicKeyBase64 = builder.Configuration["Authentication:PUBLIC_KEY"];
var privateKeyBase64 = builder.Configuration["PRIVATE_KEY"];

try
{
    if (!string.IsNullOrEmpty(publicKeyBase64))
    {
        var rsa = RSA.Create();
        // .Trim() ensures no accidental leading/trailing spaces cause a Base64 error
        byte[] keyBytes = Convert.FromBase64String(publicKeyBase64.Trim());

        // Use SubjectPublicKeyInfo for your X.509 formatted public key
        rsa.ImportSubjectPublicKeyInfo(keyBytes, out _);
        var publicKey = new RsaSecurityKey(rsa);

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
                    IssuerSigningKey = publicKey
                };
            });

        // Configure signing if the Private Key is available (loaded from User Secrets)
        if (!string.IsNullOrEmpty(privateKeyBase64))
        {
            var rsaPrivate = RSA.Create();
            byte[] privateKeyBytes = Convert.FromBase64String(privateKeyBase64.Trim());

            // Using ImportRSAPrivateKey for your PKCS#1 formatted private key
            /*
             * We had an issue with the signing credentials. The service builder was detecting
             * escape and unrecognised characters and these was as a result of the wrong method used to import
             * this private key. I was using ImportRSAPrivateKey() instead of ImportPkcs8PrivateKey().
             */
            rsaPrivate.ImportPkcs8PrivateKey(privateKeyBytes, out _);

            var signingCredentials = new SigningCredentials(
                new RsaSecurityKey(rsaPrivate),
                SecurityAlgorithms.RsaSha256);

            builder.Services.AddSingleton(signingCredentials);
        }
    }
}
catch (Exception ex)
{
    // If this hits, there is still a formatting issue in the strings themselves
    Console.WriteLine($"Warning: Authentication setup failed: {ex.Message}");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 4. MIDDLEWARE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();