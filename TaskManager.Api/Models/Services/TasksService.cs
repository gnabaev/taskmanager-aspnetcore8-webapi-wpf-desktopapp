using TaskManager.Common.Models;

namespace TaskManager.Api.Models.Services
{
    public class TasksService : ICommonService<TaskModel>
    {
        private readonly ApplicationContext _db;

        public TasksService(ApplicationContext db)
        {
            _db = db;
        }

        public bool Create(TaskModel model)
        {
            try
            {
                var newTask = new Task(model);

                _db.Tasks.Add(newTask);
                _db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            var task = _db.Tasks.FirstOrDefault(u => u.Id == id);

            if (task != null)
            {
                try
                {
                    _db.Tasks.Remove(task);
                    _db.SaveChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public TaskModel Get(int id)
        {
            var task = _db.Tasks.FirstOrDefault(p => p.Id == id);

            return task?.ToDto();
        }

        public bool Update(int id, TaskModel model)
        {
            var existingTask = _db.Tasks.FirstOrDefault(u => u.Id == id);

            if (existingTask != null)
            {
                try
                {
                    existingTask.Name = model.Name;
                    existingTask.Description = model.Description;
                    existingTask.StartDate = model.StartDate;
                    existingTask.EndDate = model.EndDate;
                    existingTask.File = model.File;
                    existingTask.Column = model.Column;
                    existingTask.ExecutorId = model.ExecutorId;

                    _db.Tasks.Update(existingTask);
                    _db.SaveChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public IQueryable<TaskModel> GetDeskTasks(int deskId)
        {
            return _db.Tasks.Where(t => t.DeskId == deskId).Select(t => t.ToDto());
        }


        public IQueryable<TaskModel> GetAll(int userId)
        {
            return _db.Tasks.Where(t => t.CreatorId == userId || t.ExecutorId == userId).Select(t => t.ToDto());
        }
    }
}
