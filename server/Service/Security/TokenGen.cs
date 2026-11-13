using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Infra.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Service.Security;

public class TokenGen(JwtOptions options)
{
    public string CreateToken(User user, string roleName)
    {
        var claims = new List<Claim> //Claims are the facts inside the token. We store the user’s id, name and role.
        { //Never put the password or hash in a token. Anyone can decode a token and read it
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, roleName),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);  //SigningCredentials with HmacSha256 signs the token with our secret key. Same key signs and verifies, so it must stay secret.

        //Never put the password or hash in a token. Anyone can decode a token and read it
        var token = new JwtSecurityToken(
            options.Issuer,
            options.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(options.ExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}