using System;

namespace school_management_backend.Models
{
    public class Entity
    {
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public Guid? DeletedBy { get; set; }
        public bool Status { get; set; }
    }
}
