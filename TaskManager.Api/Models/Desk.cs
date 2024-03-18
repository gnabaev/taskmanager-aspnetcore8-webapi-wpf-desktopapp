﻿using System.ComponentModel.DataAnnotations.Schema;

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
    }
}
