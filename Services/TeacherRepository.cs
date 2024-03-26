using Microsoft.EntityFrameworkCore;
using school_management_backend.Helpers;
using school_management_backend.Interfaces;
using school_management_backend.Models;
using System;
using BC = BCrypt.Net.BCrypt;
using System.Collections.Generic;
using System.Linq;
using school_management_backend.Enum;

namespace school_management_backend.Services
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly DBContext _context;

        public TeacherRepository(DBContext context)
        {
            _context = context;
        }

        public IEnumerable<Teacher> GetAllTeachers(Guid user_id)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserId == user_id);
            var teachers = _context.Teachers
                .Where(x => x.SchoolId == user.SchoolId)
                .Include(x => x.School)
                .Where(x => x.SchoolId == x.School.Id)
                .ToList();
            return teachers;
        }
        public bool CheckTeacherName(Teacher teacher)
        {
            var data = _context.Teachers.Any(x => x.FirstName == teacher.FirstName && x.MiddleName == teacher.MiddleName && x.LastName == teacher.LastName);
            return data;
        }
        public Teacher CreateTeacher(Teacher teacher, Guid user_id)
        {
            if(teacher.Id == Guid.Empty)
            {
                teacher.Id = Guid.NewGuid();
            }
            teacher.RoleId = Role.Teacher;
            teacher.CreatedDate = DateTime.Now;
            teacher.Password = BC.HashPassword(teacher.Password);
            _context.Teachers.Add(teacher);
            _context.SaveChanges();

            return teacher;
        }
    }
}
