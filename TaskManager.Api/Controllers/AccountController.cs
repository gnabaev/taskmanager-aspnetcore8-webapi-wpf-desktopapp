using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using TaskManager.Api.Models;
using TaskManager.Api.Models.Services;
using TaskManager.Common.Models;

namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly UsersService _usersService;

        public AccountController(ApplicationContext db)
        {
            _db = db;
            _usersService = new UsersService(db);
        }

        [HttpGet("info")]
        public IActionResult GetCurrentUserInfo()
        {
            string username = HttpContext.User.Identity.Name;
            var user = _db.Users.FirstOrDefault(u => u.Email == username);

            if (user != null)
            {
                return Ok(user.ToDto());
            }

            return NotFound();
        }

        [HttpPost("token")]
        public IActionResult GetToken()
        {
            var userData = _usersService.GetUserLoginPassFromBasicAuth(Request);
            var login = userData.Item1;
            var pass = userData.Item2;
            var identity = _usersService.GetIdentity(login, pass);

            if (identity == null)
            {
                return BadRequest("Неправильный логин или пароль");
            }

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPut("update")]
        public IActionResult UpdateUser([FromBody] UserModel userModel)
        {
            if (userModel != null)
            {
                string username = HttpContext.User.Identity.Name;
                var existingUser = _db.Users.FirstOrDefault(u => u.Email == username);

                if (existingUser != null)
                {
                    existingUser.FirstName = userModel.FirstName;
                    existingUser.LastName = userModel.LastName;
                    existingUser.Password = userModel.Password;
                    existingUser.Phone = userModel.Phone;
                    existingUser.Photo = userModel.Photo;

                    _db.Users.Update(existingUser);
                    _db.SaveChanges();

                    return Ok();
                }

                return NotFound();
            }

            return BadRequest();
        }

    }
}
