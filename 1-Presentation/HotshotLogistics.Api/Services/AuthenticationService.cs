// <copyright file="AuthenticationService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HotshotLogistics.Api.Services
{
    /// <summary>
    /// Service for handling authentication and JWT token generation.
    /// </summary>
    public class AuthenticationService
    {
        private readonly IConfiguration configuration;
        private readonly string jwtSecret;
        private readonly string jwtIssuer;
        private readonly string jwtAudience;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public AuthenticationService(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.jwtSecret = this.configuration["JWT_SECRET_KEY"] ?? throw new InvalidOperationException("JWT_SECRET_KEY not configured");
            this.jwtIssuer = this.configuration["JWT_ISSUER"] ?? throw new InvalidOperationException("JWT_ISSUER not configured");
            this.jwtAudience = this.configuration["JWT_AUDIENCE"] ?? throw new InvalidOperationException("JWT_AUDIENCE not configured");
        }

        /// <summary>
        /// Validates a JWT token.
        /// </summary>
        /// <param name="token">The token to validate.</param>
        /// <returns>The claims principal if valid, null otherwise.</returns>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(this.jwtSecret);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = this.jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = this.jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Generates a JWT token for the given user claims.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="userRole">The user role.</param>
        /// <param name="additionalClaims">Additional claims to include.</param>
        /// <returns>The generated JWT token.</returns>
        public string GenerateToken(string userId, string userRole, IEnumerable<Claim>? additionalClaims = null)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId),
                new(ClaimTypes.Role, userRole),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            if (additionalClaims != null)
            {
                claims.AddRange(additionalClaims);
            }

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.jwtSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: this.jwtIssuer,
                audience: this.jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), // Token expires in 2 hours
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}