using school_management_backend.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace school_management_backend.Models
{
    public class Users : Entity
    {
        [Key]
        public Guid UserId { get; set; }
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
