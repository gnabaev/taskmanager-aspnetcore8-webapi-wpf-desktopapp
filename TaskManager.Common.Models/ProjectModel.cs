using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Common.Models;

namespace TaskManager.Api.Models
{
    public class ProjectModel : CommonModel
    {
        public ProjectStatus Status { get; set; }

        // Реляционные отношения
        public int? AdminId { get; set; }

        public List<int>? UserIds { get; set; } = new List<int>();

        public List<int>? DeskIds { get; set; } = new List<int>();
    }
}
