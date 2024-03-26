using school_management_backend.Models;
using System;

namespace school_management_backend.Interfaces
{
    public interface IStudentService
    {
        Student GetStudentById(Guid id, string token);
        Student GetStudentByUserName(string userName);
        dynamic ReturnResult(string token, Student user);
        void AddToken(string token, Student user);
        void UpdateToken(string token);
        string generateJwtToken(Student user);
    }
}
