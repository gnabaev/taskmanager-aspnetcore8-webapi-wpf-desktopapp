namespace TaskManager.Common.Models
{
    public class ProjectModel : CommonModel
    {
        public ProjectStatus Status { get; set; }

        // Реляционные отношения
        public int? AdminId { get; set; }

        public List<int>? UserIds { get; set; } = new List<int>();

        public List<int>? DeskIds { get; set; } = new List<int>();

        public ProjectModel()
        {

        }

        public ProjectModel(string name, string description, ProjectStatus status)
        {
            Name = name;
            Description = description;
            Status = status;
        }
    }
}
