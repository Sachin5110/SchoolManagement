using school_management_backend.Models;
using System;
using System.Collections.Generic;

namespace school_management_backend.Interfaces
{
    public interface ITeacherRepository
    {
        IEnumerable<Teacher> GetAllTeachers(Guid user_id);
        Teacher CreateTeacher(Teacher teacher, Guid user_id);
        bool CheckTeacherName(Teacher teacher);
    }
}
