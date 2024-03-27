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
    public class TasksController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly UsersService _usersService;
        private readonly TasksService _tasksService;

        public TasksController(ApplicationContext db)
        {
            _db = db;
            _usersService = new UsersService(db);
            _tasksService = new TasksService(db);
        }

        [HttpGet()]
        public async Task<IEnumerable<TaskModel>> GetUserTasks()
        {
            var user = _usersService.Get(HttpContext.User.Identity.Name);

            if (user != null)
            {
                return await _tasksService.GetAll(user.Id).ToListAsync();
            }

            return Array.Empty<TaskModel>();
        }

        [HttpGet("desk")]
        public async Task<IEnumerable<TaskModel>> GetTasks(int deskId)
        {
            return await _tasksService.GetDeskTasks(deskId).ToListAsync();
        }

        [HttpGet("{id}")]
        public IActionResult GetTask(int id) 
        {
            var task = _tasksService.Get(id);

            return task != null ? Ok(task) : NotFound();
        }

        [HttpPost]
        public IActionResult CreateTask([FromBody] TaskModel taskModel)
        {
            if (taskModel != null)
            {
                var user = _usersService.Get(HttpContext.User.Identity.Name);
                if (user != null)
                {
                    taskModel.CreatorId = user.Id;

                    bool result = _tasksService.Create(taskModel);

                    return result ? Ok() : NotFound();
                }

                return Unauthorized();
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] TaskModel taskModel)
        {
            if (taskModel != null)
            {
                var user = _usersService.Get(HttpContext.User.Identity.Name);

                if (user != null)
                {
                    bool result = _tasksService.Update(id, taskModel);

                    return result ? Ok() : NotFound();
                }

                return Unauthorized();
            }

            return BadRequest();
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            bool result = _tasksService.Delete(id);
            return result ? Ok() : NotFound();
        }
    }
}
