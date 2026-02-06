using DAL.Infrastructure.Redis;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PRN232_LorKingDom.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context, IRedisService redis)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwtToken = tokenHandler.ReadJwtToken(token);
                    var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

                    // Kiểm tra token có trong blacklist không
                    if (!string.IsNullOrEmpty(jti))
                    {
                        var isBlacklisted = await redis.ExistsAsync($"blacklist:{jti}");
                        if (isBlacklisted)
                        {
                            context.Response.StatusCode = 401;
                            await context.Response.WriteAsJsonAsync(new
                            {
                                status = 401,
                                statusMessage = "UNAUTHORIZED",
                                message = "Token đã bị thu hồi"
                            });
                            return;
                        }
                    }

                    var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = _configuration["JwtSettings:Issuer"],
                        ValidAudience = _configuration["JwtSettings:Audience"],
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);
                }
                catch
                {
                    // Token không hợp lệ - tiếp tục xử lý request
                }
            }

            await _next(context);
        }
    }
}