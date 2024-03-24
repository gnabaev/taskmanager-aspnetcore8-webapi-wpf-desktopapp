using TaskManager.Common.Models;

namespace TaskManager.Api.Models
{
    public class Project : CommonObject
    {
        public int Id { get; set; }

        public ProjectStatus Status { get; set; }

        // Реляционные отношения
        public int? AdminId { get; set; }

        public ProjectAdmin? Admin { get; set; }

        public List<User>? Users { get; set; } = new List<User>();

        public List<Desk>? Desks { get; set; } = new List<Desk>();

        public Project()
        {
            
        }

        public Project(ProjectModel projectModel) : base(projectModel)
        {
            Id = projectModel.Id;
            AdminId = projectModel.AdminId;
            Status = projectModel.Status;
        }

        public ProjectModel ToDto()
        {
            return new ProjectModel()
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                CreationDate = this.CreationDate,
                Photo = this.Photo,
                AdminId = this.AdminId,
                Status = this.Status
            };
        }

    }
}
