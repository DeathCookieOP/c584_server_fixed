using AngularApp1.Server.DTO;
using DataModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace AngularApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(WorldContext context,  UserManager<AppUser> userManager, JwtHandler jwtHandler) : ControllerBase
    {

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginRequest request)
        {
            AppUser? user = await userManager.FindByNameAsync(request.Username);

            if (user == null) {
                //401 Error - Unauthorized 
                return Unauthorized("Bad username 💀");
            }

            //if we here that means that the user was found
            bool success = await userManager.CheckPasswordAsync(user, request.Password);

            //403 Error - Forbidden -> Knows who user is but isnt authorized
            if (!success) return Unauthorized("Bad Password 😲");

            JwtSecurityToken token = await jwtHandler.GetTokenAsync(user);

            string jwtString = new JwtSecurityTokenHandler().WriteToken(token);

            LoginResponse response = new()
            {
                Success = true,
                Message = "mom loves me, not you guys",
                Token = jwtString,

            };

            return Ok(response);

        }

    }
}
