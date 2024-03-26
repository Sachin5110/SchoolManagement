using school_management_backend.Interfaces;
using school_management_backend.Models;
using System.Collections.Generic;
using System;
using BC = BCrypt.Net.BCrypt;
using school_management_backend.Helpers;
using System.Linq;
using school_management_backend.Enum;

namespace school_management_backend.Services
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DBContext _context;

        public StudentRepository(DBContext context)
        {
            _context = context;
        }

        public IEnumerable<Student> GetAllStudents(Guid id)
        {
            var userData = _context.Users.Where(x => x.UserId == id).FirstOrDefault();
            var students = _context.Students.Where(x =>x.SchoolId == userData.SchoolId && x.Status && x.DeletedDate == default).ToList();
            return students;
        }
        public bool CheckStudentName(Student student)
        {
            var data = _context.Students.Any(x => x.FirstName == student.FirstName && x.MiddleName == student.MiddleName && x.LastName == student.LastName);
            return data;
        }
        public Student CreateStudent(Student student)
        {
            if(student.Id == Guid.Empty)
            {
                student.Id = Guid.NewGuid();
            }
            student.RoleId = Role.Student;
            student.Password = BC.HashPassword(student.Password);
            student.CreatedDate = DateTime.Now;
            _context.Students.Add(student);
            _context.SaveChanges();

            return student;
        }
    }
}
