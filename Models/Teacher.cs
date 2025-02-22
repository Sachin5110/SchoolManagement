﻿using school_management_backend.Enum;
using System;

namespace school_management_backend.Models
{
    public class Teacher : Entity
    {
        public Guid Id { get; set; }
        public string TeacherId { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public int PinCode { get; set; }
        public string? HighestQualification { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public Role RoleId { get; set; }
        public Roles Role { get; set; }
        public Guid SchoolId { get; set; }
        public School School { get; set; }
        public long? TokenId { get; set; }
        public virtual jwt_tokens Token { get; set; }
    }
}
