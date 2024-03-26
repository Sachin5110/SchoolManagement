using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using school_management_backend.Enum;
using school_management_backend.Models;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using school_management_backend.Helpers;
using school_management_backend.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace school_management_backend.Services
{
    public class StudentService : IStudentService
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;
        public StudentService(DBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public Student GetStudentById(Guid id, string token)
        {
            return _context.Students.Include(e => e.Token).FirstOrDefault(usr => usr.Id == id);
        }
        public Student GetStudentByUserName(string userName)
        {
            var student = _context.Students.Where(x => x.UserName == userName)
                .Include(x => x.School).Where(x => x.SchoolId == x.School.Id)
                .FirstOrDefault();
            return student;
        }
        public string generateJwtToken(Student user)
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
                      { new Claim("id", user.Id.ToString()),
                        new Claim("name", user.UserName),
                      }
                ),
                Expires = DateTime.UtcNow.AddHours(expired_time),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        public void AddToken(string token, Student user)
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
        public dynamic ReturnResult(string token, Student user)
        {
            dynamic result = new ExpandoObject();
            result = new
            {
                auth = true,
                uid = user.Id,
                schoolName = user.School.SchoolName.ToUpper(),
                userName = user.FirstName,
                roleName = user.RoleId.ToString(),
                token

            };
            return result;
        }
    }
}
