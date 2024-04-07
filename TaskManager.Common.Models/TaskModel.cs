﻿namespace TaskManager.Api.Models
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

        public TaskModel()
        {
            
        }

        public TaskModel(string name, string description, DateTime startDate, DateTime endDate, string column)
        {
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            Column = column;
        }
    }
}
