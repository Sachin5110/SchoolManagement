using Microsoft.EntityFrameworkCore;
using school_management_backend.Enum;
using school_management_backend.Helpers;
using school_management_backend.Interfaces;
using school_management_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace school_management_backend.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly DBContext _context;

        public UserRepository(DBContext context) 
        { 
            _context = context;
        }

        public IEnumerable<Users> GetAllUsers(Guid id)
        {
            var users = _context.Users
                .Where(x => x.SchoolId == x.School.Id)
                .Include(x => x.School)
                .Include(x => x.Role)
                .Where(x => x.SchoolId == x.School.Id && x.Status && x.RoleId == x.Role.Id)
                .ToList();
            return users;
        }
        public IEnumerable<Roles> GetAllRoles()
        {
            var roles = _context.Roles.ToList();
            return roles;
        }
        public Roles GetRoleById(Role id)
        {
            var role = _context.Roles.Where(x => x.Id == id).FirstOrDefault();
            return role;
        }
        
        public bool CheckRoleName(Roles role)
        {
            var data = _context.Roles.Any(x => x.Role == role.Role && x.Id == role.Id);
            return data;
        }
        public Roles CreateRole(Roles role, Guid user_id)
        {
            switch (role.Role)
            {
                case nameof(Role.GlobalAdmin):
                    role.Id = Role.GlobalAdmin;
                    break;
                case nameof(Role.SuperAdmin):
                    role.Id = Role.SuperAdmin;
                    break;
                case nameof(Role.Admin):
                    role.Id = Role.Admin;
                    break;
                case nameof(Role.Teacher):
                    role.Id = Role.Teacher;
                    break;
                case nameof(Role.Student):
                    role.Id = Role.Student;
                    break;
                default:
                    role = null; break;
            }
            if(role != null)
            {
                role.CreatedBy = user_id;
                role.CreatedDate = DateTime.Now;
                _context.Roles.Add(role);
                _context.SaveChanges();
            }
            return role;
        }
        public Roles DeleteRole(Roles role)
        {
            role.DeletedDate = DateTime.Now;
            role.Status = false;
            _context.Roles.Update(role);
            _context.SaveChanges();

            return role;
        }
    }
}
