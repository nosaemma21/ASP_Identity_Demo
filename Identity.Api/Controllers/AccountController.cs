using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Api.CustomErrors;
using Identity.Api.Data;
using Identity.Api.DTOs;
using Identity.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration
        )
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(AddUserDTO addUser)
        {
            var newUser = new AppUser { UserName = addUser.UserName, Email = addUser.Email };

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Add new user
            var result = await _userManager.CreateAsync(newUser, addUser.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("IdentityError", error.Description);
                }
                return BadRequest(ModelState);
            }

            await _userManager.AddToRoleAsync(newUser, AppRoles.User);
            //create jwt and return
            var jwt = await GenerateJwt(newUser);

            return Ok(new { name = newUser.UserName, jwt = jwt });
        }

        public async Task<string> GenerateJwt(AppUser user)
        {
            var Issuer = _configuration["Jwt:Issuer"];
            var Audience = _configuration["Jwt:Audience"];
            var Secret = _configuration["Jwt:Secret"];

            var roles = await _userManager.GetRolesAsync(user);

            if (Issuer is null || Audience is null || Secret is null)
            {
                throw new JwtException("Invalid or no JWT credentials");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
            var claims = new List<Claim> { };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            claims.Add(new Claim(ClaimTypes.Name, user.UserName!));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = Issuer,
                Audience = Audience,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
                Expires = DateTime.UtcNow.AddDays(30),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }
    };
}
