using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using school_management_backend.Helpers;
using school_management_backend.Interfaces;
using school_management_backend.Models;
using System.Dynamic;
using System;
using System.Linq;

namespace school_management_backend.Controllers
{
    [Route("/admin/")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        CommonHelper commonhelper = new CommonHelper();
        private IStudentRepository _studentRepository;
        private IMapper _mapper;

        dynamic result = new ExpandoObject();
        string requestToken = "";
        public StudentController(IStudentRepository studentRepository, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("get-all-students")]
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
            var students = _studentRepository.GetAllStudents(user_id);
            if (!String.IsNullOrEmpty(search))
            {
                bool isNumber = Guid.TryParse(search, out Guid numericValue);
                if (isNumber)
                {
                    students = students.Where(r => r.Id == numericValue);
                }
                else
                {
                    students = students.Where(r => r.FirstName.Contains(search));
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
            var count = students.Count();
            result.count = count;

            Func<dynamic, dynamic> orderingFunction = i =>
                             orderBy == "Id" ? i.Id :
                             orderBy == "Role" ? i.Role : "";

            if (orderTypeAsc)
            {
                var allstudents = students
                        .OrderBy(orderingFunction)
                        .Skip(offset)
                        .Take(final_item_per_page)
                        .ToList();
                if (allstudents == null)
                {
                    return NotFound(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status404NotFound, "Unable to fetch students.", result));
                }
                if (allstudents.Count == 0)
                {
                    result.rows = allstudents;
                    return NotFound(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status404NotFound, "No students found.", result));
                }
                result.rows = allstudents;
                return Ok(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status200OK, "students data found.", result));
            }
            else
            {
                var allstudents = students
                       .OrderByDescending(orderingFunction)
                       .Skip(offset)
                       .Take(final_item_per_page)
                       .ToList();

                if (allstudents == null)
                {
                    return NotFound(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status404NotFound, "Unable to fetch students.", result));
                }
                if (allstudents.Count == 0)
                {
                    result.rows = allstudents;
                    return NotFound(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status404NotFound, "No students found.", result));
                }
                result.rows = allstudents;
                return Ok(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status200OK, "students data found.", result));
            }
        }

        [Authorize]
        [HttpPost("create-student")]
        public IActionResult CreateStudent(Student student, Guid user_id)
        {
            var user = (Users)HttpContext.Items["User"];
            if (user.UserId != user_id)
            {
                return Unauthorized(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status401Unauthorized, "userId and jwt token do not match.", result));
            }

            var isAlreadyExists = _studentRepository.CheckStudentName(student);
            if (isAlreadyExists)
            {
                var errMsg = "Student \"" + student.FirstName + " " + student.MiddleName + " " + student.LastName + "\" is already taken";
                return BadRequest(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status400BadRequest, errMsg, result));
            }

            var user_data = _studentRepository.CreateStudent(student);
            if (user_data == null)
            {
                var errMsg = "Something went wrong while creating teacher";
                return BadRequest(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status400BadRequest, errMsg, result));
            }
            var msg = "New Teacher is created";
            return Ok(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status200OK, msg, student));
        }
    }
}
