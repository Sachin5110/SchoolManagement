using school_management_backend.Models;
using System;

namespace school_management_backend.Interfaces
{
    public interface IDashboardRepository
    {
        int CountGlobalAdmins();
        int CountSuperAdmins(Users user);
        int CountAdmins(Users user);
        int[] ReturnCount(Users user);
        int CountTeachersAndStudentUsers(Users user);
        int TotalTeachers(Users user);
        int TotalStudents(Users user);
        
    }
}
