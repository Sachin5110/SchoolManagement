using school_management_backend.Models;
using System.Collections.Generic;
using System;

namespace school_management_backend.Interfaces
{
    public interface IUserService
    {
        Users GetById(Guid id, string token);
        Users GetUserByUserName(Users User);
        Users Authenticate(Users user);
        bool CheckUser(Users user);
        dynamic ReturnResult(string token, Users user);
        Users CreateUser(Users user, Guid user_id);
        void AddToken(string token, Users user);
        void UpdateToken(string token);
        string generateJwtToken(Users user);
        Users CreateGlobalAdmin(Users user);
    }
}
