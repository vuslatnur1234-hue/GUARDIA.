
using Guardia.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Guardia.API.Services.AuthDepartman
{
    public class JWTService
    {
        private readonly IConfiguration _config;

        public JWTService(IConfiguration config)
        {
            _config = config;
        }

        public string AdminTokenUret(Admin admin)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, admin.id.ToString()),
                new Claim(ClaimTypes.Name,           admin.admin_no ?? ""),
                new Claim("departman",               admin.departman ?? ""),
                new Claim("adSoyad",                 admin.ad_soyad ?? ""),
                new Claim(ClaimTypes.Role,           "Admin")
            };

            return TokenUret(claims);
        }

        public string PersonelTokenUret(Models.PersonelGirisBilgileri pg, string adSoyad)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, pg.PersonelId.ToString()),
                new Claim(ClaimTypes.Name,           pg.SicilNo),
                new Claim("adSoyad",                 adSoyad),
                new Claim(ClaimTypes.Role,           "Personel")
            };

            return TokenUret(claims);
        }

        private string TokenUret(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var dakika = int.Parse(_config["Jwt:ExpirationMinutes"] ?? "480");

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(dakika),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
} 
