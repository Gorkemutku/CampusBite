using Identity.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Swagger / OpenAPI dökümantasyonu için gerekli servisler
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Veritabanı bağlantımızı sisteme tanıtıyoruz
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Controller altyapısını ekliyoruz
builder.Services.AddControllers();

var app = builder.Build();

// 4. Geliştirici ortamında Swagger arayüzünü (Test ekranını) aktif ediyoruz
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 5. HTTPS Yönlendirmesini kapattık (Hata almamak için yorum satırı yaptık)
// app.UseHttpsRedirection();

// 6. Yazdığımız Controller'ların rotalarını dışa açıyoruz (Eksik olan kısım)
app.MapControllers();

app.Run();