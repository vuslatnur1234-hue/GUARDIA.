using Guardia.API.Data;
using Guardia.API.Services;
using Guardia.API.Services.ai;
using Guardia.API.Services.AuthDepartman;
using Guardia.API.Services.BilgiIslem;
using Guardia.API.Services.Hukuk;
using Guardia.API.Services.IdariIsler;
using Guardia.API.Services.InsanKaynaklari;
using Guardia.API.Services.Interfaces;
using Guardia.API.Services.Personel;
using Guardia.API.Services.SatinAlma;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// --- Servis Kayýtlarý ---
builder.Services.AddScoped<IIkService, IkService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBiService, BiService>();
builder.Services.AddScoped<IHkService, HkService>();
builder.Services.AddScoped<IiiDashboardServisi, IiDashboardServisi>();
builder.Services.AddScoped<IAiServisi, AiServisi>();
builder.Services.AddScoped<IArizaServisi, ArizaServisi>();
builder.Services.AddScoped<IQrServisi, QrServisi>();
builder.Services.AddScoped<IPIzinServisi, PIzinServisi>();
builder.Services.AddScoped<IPYemekServisi, PYemekServisi>();
builder.Services.AddScoped<IPersonelGiris, PersonelGirisServisi>();
builder.Services.AddScoped<IIndexSatinAlmaServisi, SatinAlmaIndexServisi>();
builder.Services.AddScoped<IPersonelServisi, PersonelServisi>();

// Þifre Ýþlemleri (Çift yazýlan satýrlar teke düþürüldü)
builder.Services.AddScoped<IPSifreUnuttumServisi, PSifreUnuttumServisi>(); 
builder.Services.AddScoped<IPSifreUnuttumServisi, PSifreUnuttumServisi>();

// Yeni oluþturduðumuz Panel ve Profil Merkezi Servisi
builder.Services.AddScoped<IPersonelMerkeziServis, PersonelMerkeziServis>();

// JWT Servisi 
builder.Services.AddSingleton<JWTService>();

// JWT Authentication Kurulumu
var jwtKey = builder.Configuration["Jwt:Key"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero  // Token tam sürede expire olsun
    };
});

// Authorization politikalarý
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", p => p.RequireRole("Admin"))
    .AddPolicy("PersonelOnly", p => p.RequireRole("Personel"))
    .AddPolicy("Herkes", p => p.RequireAuthenticatedUser());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Politikasý
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
        opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true);



// --- Uygulama Yapýlandýrmasý ---
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
