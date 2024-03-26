using school_management_backend.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace school_management_backend.Models
{
    public class jwt_tokens
    {

        [Key]
        public long Id { get; set; }
        public string Token { get; set; }
        public string Status { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }

    }
}
