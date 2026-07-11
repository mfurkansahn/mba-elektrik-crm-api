using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using MbaCrm.Api.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MbaCrm.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> CreateTokenAsync(
            ApplicationUser user)
        {
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "Jwt:Key yapılandırması bulunamadı."
                );

            var jwtIssuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "Jwt:Issuer yapılandırması bulunamadı."
                );

            var jwtAudience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "Jwt:Audience yapılandırması bulunamadı."
                );

            var expiresInMinutes =
                _configuration.GetValue<int>("Jwt:ExpiresInMinutes");

            if (expiresInMinutes <= 0)
            {
                throw new InvalidOperationException(
                    "Jwt:ExpiresInMinutes sıfırdan büyük olmalıdır."
                );
            }

            var claims = new List<Claim>
            {
                new(
                    JwtRegisteredClaimNames.Sub,
                    user.Id
                ),
                new(
                    JwtRegisteredClaimNames.Email,
                    user.Email ?? string.Empty
                ),
                new(
                    ClaimTypes.Name,
                    user.FullName
                ),
                new(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString()
                )
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(
                    new Claim(ClaimTypes.Role, role)
                );
            }

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    expiresInMinutes
                ),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}