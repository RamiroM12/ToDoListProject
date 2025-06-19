using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ToDoListProject.Dtos;
using ToDoListProject.Interfaces;
using ToDoListProject.Models;

namespace ToDoListProject.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;

        public TokenService(IConfiguration config, UserManager<User> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<TokenDto> GenerateToken(User user, bool populateExp)
        {
            var signinCredentials = GetSigningCredentials();
            var claims = await GetClaims(user);
            var token = generateTokenOptions(signinCredentials, claims.ToList<Claim>());

            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;

            if (populateExp)
            {
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            }

            await _userManager.UpdateAsync(user);

            var accesToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenDto(accesToken, refreshToken);

        }

        public string GenerateRefreshToken()
        {
            var randonNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randonNumber);

                return Convert.ToBase64String(randonNumber);
            }
        }

        public async Task<User> GetUsersFromTokens(string accesToken)
        {
            var principal = GetPrincipalsFromExpiredTokens(accesToken);

            var user = await _userManager.FindByNameAsync(principal.Identity.Name);

            if (user == null)
            {
                throw new Exception("Error al encontrar el usuario.");
            }

            return user;
        }

        public async Task<TokenDto> RefreshToken(User user, TokenDto dto)
        {
            var principal = GetPrincipalsFromExpiredTokens(dto.AccesToken);

            user = await _userManager.FindByNameAsync(principal.Identity.Name);

            if (user == null) return null;
            
            return await GenerateToken(user, populateExp: true);
        }

        public ClaimsPrincipal GetPrincipalsFromExpiredTokens(string token)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var tokenValidParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                ValidateLifetime = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg
                .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Token");
            }

            return principal;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims(User user)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Fullname", user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            return claims;
        }

        private JwtSecurityToken generateTokenOptions(SigningCredentials credentials, List<Claim> claims)
        {
            return new JwtSecurityToken(

                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );
        }
    }
}
