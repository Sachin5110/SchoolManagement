using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using school_management_backend.Interfaces;

namespace school_management_backend.Helpers
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        public JwtMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext context, IUserService userService, ITeacherService teacherService, IStudentService studentService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                attachUserToContext(context, userService, token, teacherService, studentService);

            await _next(context);
        }

        private void attachUserToContext(HttpContext context, IUserService userService, string token, ITeacherService teacherService, IStudentService studentService)
        {
            try
            {
                var plainTextBytes = Encoding.UTF8.GetBytes(_config["JWT:JWT_SECRET_KEY"]);
                var expired_time_string = _config["JWT:JWT_EXPIRES_IN"];
                var secret_key = Convert.ToBase64String(plainTextBytes);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(secret_key);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);


                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // attach user to context on successful jwt validation
                context.Items["User"] = userService.GetById(userId, token);
                context.Items["Teacher"] = teacherService.GetTeacherById(userId, token);
                context.Items["Student"] = studentService.GetStudentById(userId, token);
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}
