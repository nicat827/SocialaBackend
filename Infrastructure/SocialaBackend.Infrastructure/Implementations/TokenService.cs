using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SocialaBackend.Application.Abstractions.Services;
using SocialaBackend.Application.Dtos;
using SocialaBackend.Domain.Entities.User;
using SocialaBackend.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SocialaBackend.Infrastructure.Implementations
{
    internal class TokenService : ITokenService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public TokenService(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<TokenResponseDto> GenerateTokensAsync(AppUser user, bool isPersistence)
        {
            ICollection<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.GivenName, user.Name),
                new Claim(ClaimTypes.Surname, user.Surname),
            };
            foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                if (await _userManager.IsInRoleAsync(user, role.ToString()))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                }
            }
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecurityKey"]));
            SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: signingCredentials

            );

            string refreshToken = Guid.NewGuid().ToString();
            
            string accessToken = tokenHandler.WriteToken(token);
            return new TokenResponseDto(
                accessToken,
                token.ValidTo,
                refreshToken,
                isPersistence ? token.ValidTo.AddDays(7) : token.ValidFrom.AddMinutes(30) 
            );
        }
    }
}
