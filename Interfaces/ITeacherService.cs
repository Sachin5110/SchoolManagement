using school_management_backend.Models;
using System;

namespace school_management_backend.Interfaces
{
    public interface ITeacherService
    {
        Teacher GetTeacherById(Guid id, string token);
        Teacher GetTeacherByUserName(string userName);
        dynamic ReturnResult(string token, Teacher user);
        void AddToken(string token, Teacher user);
        void UpdateToken(string token);
        string generateJwtToken(Teacher user);
    }
}
