using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WealthManager.Modules.Identity.Application.Interfaces;
using WealthManager.Modules.Identity.Domain.Entities;

namespace WealthManager.Modules.Identity.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(ApplicationUser user, IList<string> roles)
        {
            // 1. Get Secretkey from config

            var secretKey = _configuration["JwtSettings:secret"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // 2. Define claims (will attach in token)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 3. Create credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 4. Get expire time
            var minutes = double.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "15");
            var expires = DateTime.UtcNow.AddMinutes(minutes);

            // 5. Create token object
            var tokenHanlder = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = creds
            };

            // 6. Write token string
            var token = tokenHanlder.CreateToken(tokenDescriptor);
            return tokenHanlder.WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(string userId)
        {
            // 1. Create 64-bytes random string
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            var tokenString = Convert.ToBase64String(randomNumber);

            // 2. Get expire time 
            var days = double.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7");

            // 3. Return Entity (to save to DB later)
            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = tokenString,
                Expires = DateTime.UtcNow.AddDays(days),
                Created = DateTime.UtcNow
            };
        }
    }
}
