using school_management_backend.Enum;
using school_management_backend.Helpers;
using school_management_backend.Interfaces;
using school_management_backend.Models;
using System;
using System.Linq;

namespace school_management_backend.Services
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly DBContext _context;
        public DashboardRepository(DBContext context)
        {
            _context = context;   
        }
        public int CountAdmins(Users user)
        {
           var count = _context.Users.Where(x => x.RoleId == Role.Admin && x.Status && x.SchoolId == user.SchoolId);
            return count.Count();
        }

        public int CountGlobalAdmins()
        {
            var count = _context.Users.Where(x => x.RoleId == Role.GlobalAdmin && x.Status);
            return count.Count();
        }

        public int CountSuperAdmins(Users user)
        {
            var count = _context.Users
                    .Where(x => x.SchoolId == user.SchoolId &&
                    x.RoleId == Role.SuperAdmin);
            return count.Count();
        }

        public int CountTeachersAndStudentUsers(Users user)
        {
            var users = _context.Users
                .Where(x => x.SchoolId == user.SchoolId &&
                x.Status &&
                x.RoleId == Role.Student ||
                x.RoleId == Role.Teacher);
            return users.Count();
        }
        public int TotalTeachers(Users user)
        {
            var count = _context.Teachers
                .Where(x => x.SchoolId == user.SchoolId && x.Status);
            return count.Count();
        }
        public int TotalStudents(Users user)
        {
            var count = _context.Students
                .Where(x => x.SchoolId == user.SchoolId && x.Status);
            return count.Count();
        }

        public int[] ReturnCount(Users user)
        {
            int[] numbers = new int[6];
            var countAdmin = CountAdmins(user);
            numbers[0] = countAdmin;
            var countSuperAdmin = CountSuperAdmins(user);
            numbers[1] = countSuperAdmin;
            var countGlobalAdmin = CountGlobalAdmins();
            numbers[2] = countGlobalAdmin;
            var countUsers = CountTeachersAndStudentUsers(user);
            numbers[3] = countUsers;
            var totalTeachers = TotalTeachers(user);
            numbers[4] = totalTeachers;
            var totalStudents = TotalStudents(user);
            numbers[5] = totalStudents;
            return numbers;
        }
    }
}
