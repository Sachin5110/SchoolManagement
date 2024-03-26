using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;
using Microsoft.EntityFrameworkCore;
using school_management_backend.Models;
using school_management_backend.Enum;
using school_management_backend.Helpers;
using school_management_backend.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Dynamic;

namespace school_management_backend.Services
{
    public class UserService : IUserService
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<UserService> _logger;
        public UserService(DBContext user, IConfiguration config, ILogger<UserService> logger)
        {
            _config = config;
            _context = user;
            _logger = logger;
        }

        public Users GetById(Guid id, string token)
        {
            return _context.Users.Include(e => e.Token).FirstOrDefault(usr => usr.UserId == id);
        }

        public Users GetUserByUserName(Users User)
        {
            return _context.Users.FirstOrDefault(usr => usr.UserName == User.UserName);
        }
        public Users Authenticate(Users User)
        {
            if(User.RoleId == Role.GlobalAdmin)
            {
                var admin = _context.Users
                .Where(data => data.UserName == User.UserName)
                .FirstOrDefault();
                return admin;
            }
            var user = _context.Users
                .Where(data => data.UserName == User.UserName)
                .Include(x => x.School)
                .Where(x => x.SchoolId.Equals(x.School.Id))
                .FirstOrDefault();
            return user;
        }

        public string generateJwtToken(Users user)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(_config["JWT:JWT_SECRET_KEY"]);
            var expired_time_string = _config["JWT:JWT_EXPIRES_IN"];
            var expired_time = Convert.ToDouble(expired_time_string);

            var secret_key = Convert.ToBase64String(plainTextBytes);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret_key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                      { new Claim("id", user.UserId.ToString()),
                        new Claim("name", user.UserName),
                      }
                ),
                Expires = DateTime.UtcNow.AddHours(expired_time),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }


        public Users CreateUser(Users user, Guid user_id)
        {
            if(user.SchoolId == Guid.Empty)
            {
                _logger.LogError("School Id cannot be Empty.");
                return user = null;
            }
            var data = GetLoggedInUser(user_id);
            if(data.RoleId == Role.GlobalAdmin)
            {
                CreateNewUser(user, user_id);
            }
            if(data.RoleId == Role.SuperAdmin)
            {
                if(user.RoleId != Role.GlobalAdmin)
                {
                    CreateNewUser(user, user_id);
                }
                else
                {
                    _logger.LogError("You cannot create GlobalAdmin users.");
                    user = null;
                }
            }
            if (data.RoleId == Role.Admin)
            {
                if (user.RoleId != Role.GlobalAdmin && user.RoleId != Role.SuperAdmin)
                {
                    CreateNewUser(user, user_id);
                }
                else
                {
                    _logger.LogError("You cannot create GlobalAdmin and SuperAdmin users.");
                    user = null;
                }
            }
            return user;
        }

        public bool CheckUser(Users users)
        {
            var result = _context.Users.Any(x => x.UserName == users.UserName && x.RoleId == users.RoleId);
            return result;
        }
        public Users CreateGlobalAdmin(Users user)
        {
            user.RoleId = Role.GlobalAdmin;
            user.CreatedDate = DateTime.Now;
            user.UserId = Guid.NewGuid();
            user.Password = BC.HashPassword(user.Password);
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }
        public void AddToken(string token, Users user)
        {
            jwt_tokens tokens_data = new jwt_tokens();
            tokens_data.Token = token;
            tokens_data.Status = TokenStatus.Active.ToString();
            tokens_data.CreatedDate = DateTime.Now;

            _context.Jwt_Tokens.Add(tokens_data);
            _context.SaveChanges();

            user.TokenId = tokens_data.Id;
            _context.Update(user);
            _context.SaveChanges();
        }

        public void UpdateToken(string token)
        {
            var tokens_data = _context.Jwt_Tokens.FirstOrDefault(t => t.Token == token);
            tokens_data.Status = TokenStatus.Expired.ToString();
            tokens_data.ModifiedDate = DateTime.Now;
            tokens_data.DeletedDate = DateTime.Now;
            if (tokens_data != null)
            {
                _context.Jwt_Tokens.Update(tokens_data);
                _context.SaveChanges();
            }
        }

        public dynamic ReturnResult(string token, Users user)
        {
            dynamic result = new ExpandoObject();
            if (user.RoleId == Role.GlobalAdmin)
            {
                result = new
                {
                    auth = true,
                    uid = user.UserId,
                    schoolName = "Global Admin",
                    roleName = user.RoleId.ToString(),
                    token

                };
            }
            else
            {
                result = new
                {
                    auth = true,
                    uid = user.UserId,
                    schoolName = (user.School.SchoolName).ToUpper(),
                    roleName = user.RoleId.ToString(),
                    token
                };
            }
            return result;
        }
        private Users GetLoggedInUser(Guid userId)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserId == userId);
            return user;
        }

        private void CreateNewUser(Users user, Guid user_id)
        {
            user.CreatedBy = user_id;
            user.UserId = Guid.NewGuid();
            user.Password = BC.HashPassword(user.Password);
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}
