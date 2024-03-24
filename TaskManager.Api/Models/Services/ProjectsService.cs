namespace TaskManager.Api.Models.Services
{
    public class ProjectsService : ICommonService<ProjectModel>
    {
        private readonly ApplicationContext _db;

        public ProjectsService(ApplicationContext db)
        {
            _db = db;
        }

        public bool Create(ProjectModel model)
        {
            try
            {
                var newProject = new Project(model);

                _db.Projects.Add(newProject);
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
            var existingProject = _db.Projects.FirstOrDefault(u => u.Id == id);

            if (existingProject != null)
            {
                try
                {
                    _db.Projects.Remove(existingProject);
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

        public bool Update(int id, ProjectModel model)
        {
            var existingProject = _db.Projects.FirstOrDefault(u => u.Id == id);

            if (existingProject != null)
            {
                try
                {
                    existingProject.Name = model.Name;
                    existingProject.Description = model.Description;
                    existingProject.Status = model.Status;
                    existingProject.Photo = model.Photo;
                    existingProject.AdminId = model.AdminId;

                    _db.Projects.Update(existingProject);
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
    }
}
