using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using BC = BCrypt.Net.BCrypt;
using System.Linq;
using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using school_management_backend.Models;
using school_management_backend.Enum;
using school_management_backend.Helpers;
using school_management_backend.Interfaces;
using school_management_backend.Services;

namespace school_management_backend.Controllers
{
    [ApiController]
    [Route("/admin/")]
    public class LoginController : ControllerBase
    {
        CommonHelper commonhelper = new CommonHelper();
        private IUserService _userService;
        private IMapper _mapper;
        private readonly ITeacherService _teacherService;
        private readonly IStudentService _studentService;
        dynamic result = new ExpandoObject();
        string requestToken = "";

        public LoginController(IUserService userService, IMapper mapper, ITeacherService teacherService, IStudentService studentService)
        {
            _userService = userService;
            _mapper = mapper;
            _teacherService = teacherService;
            _studentService = studentService;
        }

        [HttpPost("login")]
        public IActionResult Authenticate(Login users)
        {
            if (!string.IsNullOrEmpty(users.UserName)
                && !string.IsNullOrEmpty(users.Password))
            {
                var user = _mapper.Map<Users>(users);
                user = _userService.GetUserByUserName(user);
                if (user == null)
                {
                    var errMsg = "Error: Invalid Username.";
                    return BadRequest(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status400BadRequest, errMsg, result));
                }
                var user_data = _userService.Authenticate(user);
                var hash_password = user_data.Password.Trim();
                if (!BC.Verify(users.Password, hash_password))
                {
                    var errMsg = "Error: Invalid Password.";
                    return BadRequest(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status400BadRequest, errMsg, result));
                }
                string token = _userService.generateJwtToken(user_data);
                if (token == null)
                {
                    var errMsg = "Login failed while generating token.";
                    return BadRequest(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status400BadRequest, errMsg, result));
                }

                _userService.AddToken(token, user_data);
                result = _userService.ReturnResult(token, user_data);
                
                return Ok(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status200OK, "Login successful.", result));
            }
            else
            {
                return UnprocessableEntity(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status422UnprocessableEntity, "Invalid request. username, password are required.", result));
            }
        }

        [HttpPost("login-teacher")]
        public IActionResult Login(Login users)
        {
            if (!string.IsNullOrEmpty(users.UserName)
                && !string.IsNullOrEmpty(users.Password))
            {
                var user = _mapper.Map<Teacher>(users);
                user = _teacherService.GetTeacherByUserName(users.UserName);
                if (user == null)
                {
                    var errMsg = "Error: Invalid Username.";
                    return BadRequest(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status400BadRequest, errMsg, result));
                }
                var hash_password = user.Password.Trim();
                if (!BC.Verify(users.Password, hash_password))
                {
                    var errMsg = "Error: Invalid Password.";
                    return BadRequest(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status400BadRequest, errMsg, result));
                }
                string token = _teacherService.generateJwtToken(user);
                if (token == null)
                {
                    var errMsg = "Login failed while generating token.";
                    return BadRequest(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status400BadRequest, errMsg, result));
                }

                _teacherService.AddToken(token, user);
                result = _teacherService.ReturnResult(token, user);

                return Ok(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status200OK, "Login successful.", result));
            }
            else
            {
                return UnprocessableEntity(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status422UnprocessableEntity, "Invalid request. username, password are required.", result));
            }
        }

        [HttpPost("login-student")]
        public IActionResult LoginStudent(Login users)
        {
            if (!string.IsNullOrEmpty(users.UserName)
                && !string.IsNullOrEmpty(users.Password))
            {
                var user = _mapper.Map<Student>(users);
                user = _studentService.GetStudentByUserName(users.UserName);
                if (user == null)
                {
                    var errMsg = "Error: Invalid Username.";
                    return BadRequest(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status400BadRequest, errMsg, result));
                }
                var hash_password = user.Password.Trim();
                if (!BC.Verify(users.Password, hash_password))
                {
                    var errMsg = "Error: Invalid Password.";
                    return BadRequest(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status400BadRequest, errMsg, result));
                }
                string token = _studentService.generateJwtToken(user);
                if (token == null)
                {
                    var errMsg = "Login failed while generating token.";
                    return BadRequest(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status400BadRequest, errMsg, result));
                }

                _studentService.AddToken(token, user);
                result = _studentService.ReturnResult(token, user);

                return Ok(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status200OK, "Login successful.", result));
            }
            else
            {
                return UnprocessableEntity(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status422UnprocessableEntity, "Invalid request. username, password are required.", result));
            }
        }

        [HttpPost("create-global-admin")]
        public IActionResult CreateGlobalAdmin(Users users)
        {
                var userAlreadyExist = _userService.CheckUser(users);
                if (userAlreadyExist)
                {
                    var errMsg = "Username \"" + users.UserName + "\" is already taken";
                    return BadRequest(commonhelper.GenerateApiResponse(requestToken, 400, errMsg, result));
                }

                var user_data = _userService.CreateGlobalAdmin(users);
                if (user_data == null)
                {
                    var errMsg = "Something went wrong while creating user";
                    return BadRequest(commonhelper.GenerateApiResponse(requestToken, 400, errMsg, result));
                }
                var msg = "Global Admin is created";
                return Ok(commonhelper.GenerateApiResponse(requestToken, 200, msg, users));
        }

        [Authorize]
        [HttpPost("create-user")]
        public IActionResult CreateUser(Users users, Guid user_id)
        {
            var user = (Users)HttpContext.Items["User"];
            if (user.UserId != user_id)
            {
                return Unauthorized(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status401Unauthorized, "userId and jwt token do not match.", result));
            }
            if (user.RoleId == Role.GlobalAdmin || user.RoleId == Role.SuperAdmin || user.RoleId == Role.Admin)
            {
                var userAlreadyExist = _userService.CheckUser(users);
                if (userAlreadyExist)
                {
                    var errMsg = "Username \"" + users.UserName + "\" is already taken";
                    return BadRequest(commonhelper.GenerateApiResponse(requestToken, 400, errMsg, result));
                }

                var user_data = _userService.CreateUser(users, user_id);
                if (user_data == null)
                {
                    var errMsg = "Something went wrong while creating user";
                    return BadRequest(commonhelper.GenerateApiResponse(requestToken, 400, errMsg, result));
                }
                var msg = "New User is created";
                return Ok(commonhelper.GenerateApiResponse(requestToken, 200, msg, users));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status500InternalServerError, "Access Denied", result));
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            _userService.UpdateToken(token);
            var msg = "Log out successful";

            result.request_token = "";
            return Ok(commonhelper.GenerateApiResponse(requestToken, 200, msg, result));
        }

        [TeacherAuthorize]
        [HttpPost("logout-teacher")]
        public IActionResult LogoutTeacher()
        {
            string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            _userService.UpdateToken(token);
            var msg = "Log out successful";

            result.request_token = "";
            return Ok(commonhelper.GenerateApiResponse(requestToken, 200, msg, result));
        }

        [StudentAuthorize]
        [HttpPost("logout-student")]
        public IActionResult LogoutStudent()
        {
            string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            _userService.UpdateToken(token);
            var msg = "Log out successful";

            result.request_token = "";
            return Ok(commonhelper.GenerateApiResponse(requestToken, 200, msg, result));
        }
    }
}
