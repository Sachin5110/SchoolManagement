using System.ComponentModel.DataAnnotations;
using System;

namespace school_management_backend.Models
{
    public class School : Entity
    {
        [Key]
        public Guid Id { get; set; }
        public string SchoolName { get; set; }
        public string SchoolAddress { get; set; }
    }
}
