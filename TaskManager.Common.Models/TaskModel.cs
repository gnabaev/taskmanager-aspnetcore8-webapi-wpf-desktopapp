namespace TaskManager.Api.Models
{
    public class TaskModel : CommonModel
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public byte[]? File { get; set; }

        public string? Column { get; set; }

        // Реляционные отношения
        public int DeskId { get; set; }

        public int? CreatorId { get; set; }

        public int? ExecutorId { get; set; }
    }
}
