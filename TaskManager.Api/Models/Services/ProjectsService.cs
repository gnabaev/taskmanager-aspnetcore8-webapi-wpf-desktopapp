using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TaskManager.Common.Models;

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

        public ProjectModel Get(int id)
        {
            var project =_db.Projects.Include(p => p.Users).FirstOrDefault(p => p.Id == id);

            var projectModel = project?.ToDto();

            if (projectModel != null)
            {
                projectModel.UserIds = project.Users.Select(u => u.Id).ToList();
            }

            return projectModel;
        }

        public async Task<IEnumerable<ProjectModel>> GetByUserId(int userId)
        {
            List<ProjectModel> result = new List<ProjectModel>();

            var admin = _db.ProjectAdmins.FirstOrDefault(a => a.UserId == userId);

            if (admin != null)
            {
                var projectsAsAdmin = await _db.Projects.Where(p => p.AdminId == admin.Id).Select(p => p.ToDto()).ToListAsync();
                result.AddRange(projectsAsAdmin);
            }

            var projectsAsUser = await _db.Projects.Include(p => p.Users).Where(p => p.Users.Any(u => u.Id == userId)).Select(p => p.ToDto()).ToListAsync();
            result.AddRange(projectsAsUser);

            return result;
        }

        public IQueryable<ProjectModel> GetAll()
        {
            return _db.Projects.Select(p => p.ToDto());
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

        public void AddUsersToProject(int id, List<int> userIds)
        {
            var project = _db.Projects.FirstOrDefault(p => p.Id == id);

            foreach (int userId in userIds)
            {
                var user = _db.Users.FirstOrDefault(u => u.Id == userId);
                project.Users.Add(user);
            }

            _db.SaveChanges();
        }

        public void RemoveUsersFromProject(int id, List<int> userIds)
        {
            var project = _db.Projects.Include(p => p.Users).FirstOrDefault(p => p.Id == id);

            foreach (int userId in userIds)
            {
                var user = _db.Users.FirstOrDefault(u => u.Id == userId);

                if (project.Users.Contains(user))
                {
                    project.Users.Remove(user);
                }
            }

            _db.SaveChanges();
        }
    }
}
