using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using school_management_backend.Helpers;
using school_management_backend.Models;
using System.Web.Http.Results;
using System;
using AutoMapper;
using school_management_backend.Interfaces;
using System.Dynamic;
using school_management_backend.Services;
using System.Linq;

namespace school_management_backend.Controllers
{
    [Route("/admin/")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        CommonHelper commonhelper = new CommonHelper();
        private ITeacherRepository _teacherRepository;
        private IMapper _mapper;

        dynamic result = new ExpandoObject();
        string requestToken = "";
        public TeacherController(ITeacherRepository teacherRepository, IMapper mapper)
        {
            _teacherRepository = teacherRepository;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("get-all-teachers")]
        public IActionResult GetUsers([FromQuery] Guid user_id, [FromQuery] string search, [FromQuery] string page, [FromQuery] string per_page, [FromQuery] string sort_by, [FromQuery] string sort_type)
        {
            var user = (Users)HttpContext.Items["User"];
            if (user.UserId != user_id)
            {
                return Unauthorized(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status401Unauthorized, "userId and jwt token do not match.", result));
            }

            int final_page = 1;
            if (!String.IsNullOrEmpty(page))
            {
                final_page = Int16.Parse(page);
            }

            int final_item_per_page = 25;
            if (!String.IsNullOrEmpty(per_page))
            {
                final_item_per_page = Int16.Parse(per_page);
            }
            int offset = 0;
            if (final_page > 1)
            {
                offset = (final_page - 1) * final_item_per_page;
            }
            var teachers = _teacherRepository.GetAllTeachers(user_id);
            if (!String.IsNullOrEmpty(search))
            {
                bool isNumber = Guid.TryParse(search, out Guid numericValue);
                if (isNumber)
                {
                    teachers = teachers.Where(r => r.Id == numericValue);
                }
                else
                {
                    teachers = teachers.Where(r => r.FirstName.Contains(search));
                }
            }
            var orderBy = "Id";
            if (!String.IsNullOrEmpty(sort_by))
            {
                orderBy = sort_by;
            }
            bool orderTypeAsc = true;
            if (!String.IsNullOrEmpty(sort_type))
            {
                if (sort_type == "desc")
                {
                    orderTypeAsc = false;
                }
            }
            var count = teachers.Count();
            result.count = count;

            Func<dynamic, dynamic> orderingFunction = i =>
                             orderBy == "Id" ? i.Id :
                             orderBy == "Role" ? i.Role : "";

            if (orderTypeAsc)
            {
                var allteachers = teachers
                        .OrderBy(orderingFunction)
                        .Skip(offset)
                        .Take(final_item_per_page)
                        .ToList();
                if (allteachers == null)
                {
                    return NotFound(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status404NotFound, "Unable to fetch Teachers.", result));
                }
                if (allteachers.Count == 0)
                {
                    result.rows = allteachers;
                    return NotFound(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status404NotFound, "No Teachers found.", result));
                }
                result.rows = allteachers;
                return Ok(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status200OK, "Teachers data found.", result));
            }
            else
            {
                var allteachers = teachers
                       .OrderByDescending(orderingFunction)
                       .Skip(offset)
                       .Take(final_item_per_page)
                       .ToList();

                if (allteachers == null)
                {
                    return NotFound(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status404NotFound, "Unable to fetch Teachers.", result));
                }
                if (allteachers.Count == 0)
                {
                    result.rows = allteachers;
                    return NotFound(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status404NotFound, "No Teachers found.", result));
                }
                result.rows = allteachers;
                return Ok(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status200OK, "Teachers data found.", result));
            }
        }

        [Authorize]
        [HttpPost("create-teacher")]
        public IActionResult CreateTeacher(Teacher teacher, Guid user_id)
        {
            var user = (Users)HttpContext.Items["User"];
            if (user.UserId != user_id)
            {
                return Unauthorized(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status401Unauthorized, "userId and jwt token do not match.", result));
            }

            var isAlreadyExists = _teacherRepository.CheckTeacherName(teacher);
            if (isAlreadyExists)
            {
                var errMsg = "Teacher \"" + teacher.FirstName +" "+ teacher.MiddleName +" "+ teacher.LastName + "\" is already taken";
                return BadRequest(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status400BadRequest, errMsg, result));
            }

            var user_data = _teacherRepository.CreateTeacher(teacher, user_id);
            if (user_data == null)
            {
                var errMsg = "Something went wrong while creating teacher";
                return BadRequest(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status400BadRequest, errMsg, result));
            }
            var msg = "New Teacher is created";
            return Ok(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status200OK, msg, teacher));
        }
    }
}
