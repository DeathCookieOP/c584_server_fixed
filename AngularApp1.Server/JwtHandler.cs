using DataModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AngularApp1.Server
{
    public class JwtHandler(IConfiguration configuration, UserManager<AppUser> userManager)
    {

        //This is part of OpenID connect std; generating token
        public async Task<JwtSecurityToken> GetTokenAsync(AppUser user) => 
            new(
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            claims: await GetClaimsAsync(user), //permissions that the owner of token provides
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(configuration["JwtSettings:ExpirationTimeInMinutes"])),
            signingCredentials: GetSigningCredentials());

        //SigningCredentials is a MicrosoftIdentity class
        private SigningCredentials GetSigningCredentials()
        {
            //key on server that prevents user on client side from tampering with token
            //key is used to generate token (kinda like encrypting)
            byte[] key = Encoding.UTF8.GetBytes(configuration["JwtSettings:SecurityKey"]!); 
            SymmetricSecurityKey secret = new(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<IEnumerable<Claim>> GetClaimsAsync(AppUser user)
        {
            List<Claim> claims = [new Claim(ClaimTypes.Name, user.UserName!)];
            //GetRolesAsync (microsoft function) -> Gets data from ASPNet table
            claims.AddRange(from role in await userManager.GetRolesAsync(user) select new Claim(ClaimTypes.Role, role));
            return claims;
        }

    }
}
