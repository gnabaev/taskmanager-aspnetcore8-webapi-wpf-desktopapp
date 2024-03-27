using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TaskManager.Api.Models.Services
{
    public class DesksService : ICommonService<DeskModel>
    {
        private readonly ApplicationContext _db;

        public DesksService(ApplicationContext db)
        {
            _db = db;
        }

        public bool Create(DeskModel model)
        {
            try
            {
                var newDesk = new Desk(model);

                _db.Desks.Add(newDesk);
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
            var existingDesk = _db.Desks.FirstOrDefault(u => u.Id == id);

            if (existingDesk != null)
            {
                try
                {
                    _db.Desks.Remove(existingDesk);
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

        public DeskModel Get(int id)
        {
            var desk = _db.Desks.Include(d => d.Tasks).FirstOrDefault(x => x.Id == id);
            var deskModel = desk?.ToDto();

            if (deskModel != null)
            {
                deskModel.TaskIds = desk?.Tasks.Select(t => t.Id).ToList();
            }

            return deskModel;
        }

        public bool Update(int id, DeskModel model)
        {
            var existingDesk = _db.Desks.FirstOrDefault(d => d.Id == id);

            if (existingDesk != null)
            {
                try
                {
                    existingDesk.Name = model.Name;
                    existingDesk.Description = model.Description;
                    existingDesk.Photo = model.Photo;
                    existingDesk.AdminId = model.AdminId;
                    existingDesk.IsPrivate = model.IsPrivate;
                    existingDesk.Columns = JsonConvert.SerializeObject(model.Columns);

                    _db.Desks.Update(existingDesk);
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

        public IQueryable<DeskModel> GetAll(int userId)
        {
            return _db.Desks.Where(d => d.AdminId == userId).Select(d => d.ToDto());
        }

        public IQueryable<DeskModel> GetProjectDesks(int projectId, int userId)
        {
            return _db.Desks.Where(d => (d.ProjectId == projectId && (d.AdminId == userId || d.IsPrivate == false))).Select(d => d.ToDto());
        }
    }
}
