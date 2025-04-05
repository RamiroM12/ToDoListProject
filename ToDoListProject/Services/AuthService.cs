using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ToDoListProject.Dtos;
using ToDoListProject.Dtos.UserDtos;
using ToDoListProject.Interfaces;
using ToDoListProject.Models;

namespace ToDoListProject.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _Configuration;
        private readonly UserManager<User> _UserManager;
        private readonly ILogger<AuthService> _logger;
        private User? user;

        public AuthService(IConfiguration configuration, UserManager<User> userManager, ILogger<AuthService> logger)
        {
            _Configuration = configuration;
            _UserManager = userManager;
            _logger = logger;
        }

        async public Task<IdentityUser> RegisterUser(RegisterUserDto dto)
        {
            var existingUser = await _UserManager.FindByEmailAsync(dto.Email);

            if(existingUser != null)
            {
                return null;
            }

            user = new User() { Email = dto.Email, FullName = dto.FullName, UserName = dto.Username };

            var result = await _UserManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return null;
            }

            return (User)user;
           
        }

        public async Task<bool> ValidateUser(LoginUserDto dto)
        {
            var findUser = await _UserManager.FindByEmailAsync(dto.Email);

            if(findUser != null && await _UserManager.CheckPasswordAsync(findUser, dto.Password))
            {
                user = findUser;

                return true;
            }

            return false;
        }

        public async Task<TokenDto> GenerateToken( bool populateExp)
        {
            var signInCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var token = generateTokenOptions(signInCredentials, claims.ToList<Claim>());


            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;

            if (populateExp)
            {
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            }

            await _UserManager.UpdateAsync(user);

            var accesToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenDto(accesToken, refreshToken);
        }

        private JwtSecurityToken generateTokenOptions(SigningCredentials credentials, List<Claim> claims)
        {
            return new JwtSecurityToken(

                issuer: _Configuration["Jwt:Issuer"],
                audience: _Configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration["Jwt:Key"]));

            return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims() {
          
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Fullname", user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userClaims = await _UserManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            return claims;
        }


        private string GenerateRefreshToken()
        {
            var randonNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randonNumber);

                return Convert.ToBase64String(randonNumber);
            }
        }
         
        public async Task<TokenDto> RefreshToken(TokenDto dto)
        {
            var principal = GetPrincipalFromExpiredToken(dto.AccesToken);

            foreach (var claim in principal.Claims)
            {
                _logger.LogInformation($"Claim Type: {claim.Type}, Value: {claim.Value}");
            }


            user = await _UserManager.FindByNameAsync(principal.Identity.Name);


            if (user == null)
            {
                _logger.LogWarning($"Usuario {principal.Identity.Name} no encontrado.");
                return null;
            }



            return await GenerateToken(populateExp: true);
        }

        public void SetTokensInsideCookie(TokenDto TokenDto, HttpContext context)
        {
            context.Response.Cookies.Append("accesToken", TokenDto.AccesToken,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddHours(2),
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });

            context.Response.Cookies.Append("refreshToken", TokenDto.RefreshToken,
               new CookieOptions
               {
                   Expires = DateTimeOffset.UtcNow.AddDays(7),
                   HttpOnly = true,
                   IsEssential = true,
                   Secure = true,
                   SameSite = SameSiteMode.None
               });
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _Configuration.GetSection("Jwt");
            var tokenValidParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration["Jwt:Key"])),
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

        public async Task<User> getUserFromToken(string accesToken)
        {
            var principal = GetPrincipalFromExpiredToken(accesToken);

            var user = await _UserManager.FindByNameAsync(principal.Identity.Name);

            if (user == null)
            {
                throw new Exception("Error al encontrar el usuario.");
            }

            return user;
        }
    }
}
