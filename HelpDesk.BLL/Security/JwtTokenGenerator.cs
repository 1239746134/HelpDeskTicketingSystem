using HelpDesk.BLL.Settings;
using HelpDesk.Models.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace HelpDesk.BLL.Security
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings _settings;

        public JwtTokenGenerator(IOptions<JwtSettings> options)
        {
            this._settings = options.Value;
        }
        public string GenerateToken(User user)
        {
            //声明 Claims
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  // → "sub"
                new Claim(ClaimTypes.Name, user.UserName),                  // → "name"
                new Claim(ClaimTypes.Role, user.Role.ToString())            // → "role"
            };

            //对称密钥 + 签名凭据
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //令牌描述
            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
                signingCredentials: credentials);

            //序列化为 JWT 字符串
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
