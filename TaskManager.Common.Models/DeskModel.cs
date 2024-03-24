using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Api.Models
{
    public class DeskModel : CommonModel
    {
        public bool IsPrivate { get; set; }

        public string[]? Columns { get; set; }

        // Реляционные отношения
        public int AdminId { get; set; }

        public int ProjectId { get; set; }

        public List<TaskModel> Tasks { get; set; } = new List<TaskModel>();
    }
}
