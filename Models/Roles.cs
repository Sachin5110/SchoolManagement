using school_management_backend.Enum;

namespace school_management_backend.Models
{
    public class Roles : Entity
    {
        public Role Id { get; set; }
        public string Role { get; set; }       
    }
}
