﻿namespace TaskManager.Common.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Phone { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public byte[]? Photo { get; set; }

        public UserStatus Status { get; set; }

        public UserModel() { }

        public UserModel(string firstName, string lastName, string email, string password, UserStatus status = UserStatus.User,
    string phone = null)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            Phone = phone;
            RegistrationDate = DateTime.Now;
            Status = status;
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
