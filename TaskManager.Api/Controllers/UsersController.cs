using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;
using TaskManager.Api.Models.Services;
using TaskManager.Common.Models;

namespace TaskManager.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly UsersService _usersService;

        public UsersController(ApplicationContext db)
        {
            _db = db;
            _usersService = new UsersService(db);
        }

        [HttpGet]
        public async Task<IEnumerable<UserModel>> GetUsers()
        {
            return await _db.Users.Select(u => u.ToDto()).ToListAsync();
        }

        [HttpGet("{id}")]
        public ActionResult<UserModel> GetUser(int id)
        {
            var user = _usersService.Get(id);
            return user != null ? Ok(user) : NotFound();
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] UserModel userModel)
        {
            if (userModel != null)
            {
                bool result = _usersService.Create(userModel);
                return result ? Ok() : NotFound();
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserModel userModel)
        {
            if (userModel != null)
            {
                bool result = _usersService.Update(id, userModel);
                return result ? Ok() : NotFound();
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            bool result = _usersService.Delete(id);
            return result ? Ok() : NotFound();
        }
    }
}
