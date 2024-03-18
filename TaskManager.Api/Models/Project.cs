using System.ComponentModel.DataAnnotations.Schema;

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
    }
}
