using school_management_backend.Models;
using System;
using System.Collections.Generic;

namespace school_management_backend.Interfaces
{
    public interface IStudentRepository
    {
        IEnumerable<Student> GetAllStudents(Guid id);
        Student CreateStudent(Student teacher);
        bool CheckStudentName(Student teacher);
    }
}
