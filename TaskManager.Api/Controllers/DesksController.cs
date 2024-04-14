using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;
using TaskManager.Api.Models.Services;
using TaskManager.Common.Models;

namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DesksController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly UsersService _usersService;
        private readonly DesksService _desksService;

        public DesksController(ApplicationContext db)
        {
            _db = db;
            _usersService = new UsersService(db);
            _desksService = new DesksService(db);
        }

        [HttpGet]
        public async Task<IEnumerable<DeskModel>> GetUserDesks()
        {
            var user = _usersService.Get(HttpContext.User.Identity.Name);

            if (user != null)
            {
                return await _desksService.GetAll(user.Id).ToListAsync();
            }

            return Array.Empty<DeskModel>();
        }

        [HttpGet("{id}")]
        public IActionResult GetDesk(int id)
        {
            var desk = _desksService.Get(id);

            return desk != null ? Ok(desk) : NotFound();
        }

        [HttpGet("project")]
        public async Task<IEnumerable<DeskModel>> GetProjectDesks(int projectId)
        {
            var user = _usersService.Get(HttpContext.User.Identity.Name);

            if (user != null)
            {
                return await _desksService.GetProjectDesks(projectId, user.Id).ToListAsync();
            }

            return Array.Empty<DeskModel>();
        }

        [HttpPost]
        public IActionResult CreateDesk([FromBody] DeskModel deskModel)
        {
            if (deskModel != null)
            {
                var user = _usersService.Get(HttpContext.User.Identity.Name);
                if (user != null)
                {
                    deskModel.AdminId = user.Id;

                    bool result = _desksService.Create(deskModel);
                    return result ? Ok() : NotFound();
                }

                return Unauthorized();
            }

            return BadRequest();

        }

        [HttpPut("{id}")]
        public IActionResult UpdateDesk(int id, [FromBody] DeskModel deskModel)
        {
            if (deskModel != null)
            {
                var user = _usersService.Get(HttpContext.User.Identity.Name);

                if (user != null)
                {
                    bool result = _desksService.Update(id, deskModel);
                    return result ? Ok() : NotFound();
                }

                return Unauthorized();
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDesk(int id)
        {
            bool result = _desksService.Delete(id);
            return result ? Ok() : NotFound();
        }
    }
}
