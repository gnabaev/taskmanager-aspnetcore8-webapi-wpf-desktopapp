using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;
using TaskManager.Common.Models;

namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _db;

        public UsersController(ApplicationContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IEnumerable<UserModel>> GetUsers()
        {
            return await _db.Users.Select(u => u.ToDto()).ToListAsync();
        }

        [HttpGet("{id}")]
        public UserModel GetUser(int id)
        {
            return _db.Users.FirstOrDefault(u => u.Id == id).ToDto();
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] UserModel userModel)
        {
            if (userModel != null)
            {
                var newUser = new User(userModel.FirstName, userModel.LastName, userModel.Email, userModel.Password,
                    userModel.Status, userModel.Phone, userModel.Photo);

                _db.Users.Add(newUser);
                _db.SaveChanges();

                return Ok();
            }

            return BadRequest();
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserModel userModel)
        {
            if (userModel != null)
            {
                var existingUser = _db.Users.FirstOrDefault(u => u.Id == id);

                if (existingUser != null)
                {
                    existingUser.FirstName = userModel.FirstName;
                    existingUser.LastName = userModel.LastName;
                    existingUser.Email = userModel.Email;
                    existingUser.Password = userModel.Password;
                    existingUser.Phone = userModel.Phone;
                    existingUser.Photo = userModel.Photo;
                    existingUser.Status = userModel.Status;

                    _db.Users.Update(existingUser);
                    _db.SaveChanges();

                    return Ok();
                }

                return NotFound();
            }

            return BadRequest();
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var existingUser = _db.Users.FirstOrDefault(u => u.Id == id);

            if (existingUser != null)
            {
                _db.Users.Remove(existingUser);
                _db.SaveChanges();

                return Ok();
            }

            return NotFound();
        }
    }
}
