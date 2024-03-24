using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Common.Models;

namespace TaskManager.Api.Models
{
    public class ProjectModel : CommonModel
    {
        public ProjectStatus Status { get; set; }

        // Реляционные отношения
        public int? AdminId { get; set; }

        public List<UserModel>? Users { get; set; } = new List<UserModel>();

        public List<DeskModel>? Desks { get; set; } = new List<DeskModel>();
    }
}
