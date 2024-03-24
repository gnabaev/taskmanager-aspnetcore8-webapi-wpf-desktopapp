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
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly UsersService _usersService;
        private readonly ProjectsService _projectsService;

        public ProjectsController(ApplicationContext db)
        {
            _db = db;
            _usersService = new UsersService(db);
            _projectsService = new ProjectsService(db);
        }

        [HttpGet]
        public async Task<IEnumerable<ProjectModel>> Get()
        {
            var user = _usersService.Get(HttpContext.User.Identity.Name);

            if (user.Status == UserStatus.Admin)
            {
                return await _projectsService.GetAll().ToListAsync();
            }
            else
            {
                return await _projectsService.GetByUserId(user.Id);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var project = _projectsService.Get(id);

            return project != null ? Ok(project) : NotFound();
        }

        [HttpPost]
        public IActionResult Create([FromBody] ProjectModel projectModel)
        {
            if (projectModel != null)
            {
                var user = _usersService.Get(HttpContext.User.Identity.Name);
                if (user != null)
                {
                    if(user.Status == UserStatus.Admin || user.Status == UserStatus.Editor)
                    {
                        var admin = _db.ProjectAdmins.FirstOrDefault(a => a.UserId == user.Id);

                        if (admin == null)
                        {
                            admin = new ProjectAdmin(user);
                            _db.ProjectAdmins.Add(admin);
                        }

                        projectModel.AdminId = admin.Id;

                        bool result = _projectsService.Create(projectModel);

                        return result ? Ok() : NotFound();
                    }
                }

                return Unauthorized();
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ProjectModel projectModel)
        {
            if (projectModel != null)
            {
                var user = _usersService.Get(HttpContext.User.Identity.Name);

                if (user != null)
                {
                    if (user.Status == UserStatus.Admin || user.Status == UserStatus.Editor)
                    {
                        bool result = _projectsService.Update(id, projectModel);
                        return result ? Ok() : NotFound();
                    }
                }

                return Unauthorized();
            }

            return BadRequest();
        }

        [HttpPut("{id}/users")]
        public IActionResult AddUsersToProject(int id, [FromBody] List<int> userIds)
        {
            if (userIds != null)
            {
                var user = _usersService.Get(HttpContext.User.Identity.Name);

                if (user != null)
                {
                    if (user.Status == UserStatus.Admin || user.Status == UserStatus.Editor)
                    {
                        _projectsService.AddUsersToProject(id, userIds);
                        return Ok();
                    }
                }

                return Unauthorized();
            }

            return BadRequest();
        }

        [HttpPut("{id}/users/remove")]
        public IActionResult RemoveUsersFromProject(int id, [FromBody] List<int> userIds)
        {
            if (userIds != null)
            {
                var user = _usersService.Get(HttpContext.User.Identity.Name);

                if (user != null)
                {
                    if (user.Status == UserStatus.Admin || user.Status == UserStatus.Editor)
                    {
                        _projectsService.RemoveUsersFromProject(id, userIds);
                        return Ok();
                    }
                }

                return Unauthorized();
            }

            return BadRequest();
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool result = _projectsService.Delete(id);
            return result ? Ok() : NotFound();
        }
    }
}
