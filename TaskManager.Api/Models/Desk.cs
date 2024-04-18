using Newtonsoft.Json;
using TaskManager.Common.Models;

namespace TaskManager.Api.Models
{
    public class Desk : CommonObject
    {
        public int Id { get; set; }

        public bool IsPrivate { get; set; }

        public string? Columns { get; set; }

        // Реляционные отношения
        public int AdminId { get; set; }

        public User? Admin { get; set; }

        public int ProjectId { get; set; }

        public Project? Project { get; set; }

        public List<Task> Tasks { get; set; } = new List<Task>();

        public Desk()
        {

        }

        public Desk(DeskModel deskModel) : base(deskModel)
        {
            Id = deskModel.Id;
            AdminId = deskModel.AdminId;
            IsPrivate = deskModel.IsPrivate;
            ProjectId = deskModel.ProjectId;
            Photo = deskModel.Photo;

            if (deskModel.Columns != null && deskModel.Columns.Any())
            {
                Columns = JsonConvert.SerializeObject(deskModel.Columns);
            }
        }

        public DeskModel ToDto()
        {
            return new DeskModel()
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                CreationDate = this.CreationDate,
                Photo = this.Photo,
                AdminId = this.AdminId,
                ProjectId = this.ProjectId,
                IsPrivate = this.IsPrivate,
                Columns = JsonConvert.DeserializeObject<string[]>(this.Columns)
            };
        }
    }
}
