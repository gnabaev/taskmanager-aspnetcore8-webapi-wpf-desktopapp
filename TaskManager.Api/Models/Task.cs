using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Api.Models
{
    public class Task : CommonObject
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public byte[]? File { get; set; }

        public string? Column { get; set; }

        // Реляционные отношения
        public int DeskId { get; set; }

        public Desk? Desk { get; set; }

        public int? CreatorId { get; set; }

        public User? Creator { get; set; }

        public int? ExecutorId { get; set; }

        public Task()
        {

        }

        public Task(TaskModel taskModel) : base(taskModel)
        {
            Id = taskModel.Id;
            Name = taskModel.Name;
            Description = taskModel.Description;
            StartDate = taskModel.CreationDate; 
            EndDate = taskModel.EndDate;
            File = taskModel.File;
            Column = taskModel.Column;
            CreatorId = taskModel.CreatorId;
            ExecutorId = taskModel.ExecutorId;
            DeskId = taskModel.DeskId;
        }

        public TaskModel ToDto()
        {
            return new TaskModel()
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                StartDate = this.CreationDate, 
                EndDate = this.EndDate,
                File = this.File,
                Column = this.Column,
                CreatorId = this.CreatorId,
                ExecutorId = this.ExecutorId,
                DeskId = this.DeskId
            };
        }
    }
}
