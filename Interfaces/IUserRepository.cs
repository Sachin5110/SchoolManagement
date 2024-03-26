using school_management_backend.Enum;
using school_management_backend.Models;
using System;
using System.Collections.Generic;

namespace school_management_backend.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<Users> GetAllUsers(Guid id);
        IEnumerable<Roles> GetAllRoles();
        Roles GetRoleById(Role id);
        Roles CreateRole(Roles role, Guid user_id);
        bool CheckRoleName(Roles role);
        Roles DeleteRole(Roles role);

    }
}
