
using Exchange.API.Data;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Exchange.API.Models;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabanı Bağlantısı
builder.Services.AddDbContext<ExchangeDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"))
);

// 2. JWT Kimlik Doğrulama
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

var secretKey = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]!);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(secretKey),

            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],

            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// 3. Swagger'a Kimlik Sorma (Authorize) Yeteneği Kazandırma
builder.Services.AddSwaggerGen(c =>
{
    // 1. Şema tanımını yapıyoruz
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Description = "JWT Token'ınızı 'Bearer {token}' formatında giriniz. (Örn: Bearer eyJhbG...)",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    };

    c.AddSecurityDefinition("Bearer", jwtSecurityScheme);

    // 2. DİKKAT: .NET 10 constructor kuralına göre nesneyi parametrelerle oluşturuyoruz
    var schemeReference = new OpenApiSecuritySchemeReference("Bearer", null, null);

    // 3. Gereksinimi lambda fonksiyonu ile bağlıyoruz
    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        { schemeReference, new List<string>() }
    });
});

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

// Middleware
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// Controller Map
app.MapControllers();

app.Run();

