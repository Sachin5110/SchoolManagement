using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using school_management_backend.Enum;
using school_management_backend.Helpers;
using school_management_backend.Interfaces;
using school_management_backend.Models;
using System;
using System.Dynamic;
using System.Linq;

namespace school_management_backend.Controllers
{
    [Route("/admin/")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        CommonHelper commonhelper = new CommonHelper();
        private IUserRepository _userRepository;
        private IMapper _mapper;

        dynamic result = new ExpandoObject();
        string requestToken = "";
        public RolesController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("get-all-roles")]
        public IActionResult GetUsers([FromQuery] Guid user_id, [FromQuery] string search, [FromQuery] string page, [FromQuery] string per_page, [FromQuery] string sort_by, [FromQuery] string sort_type)
        {
            var user = (Users)HttpContext.Items["User"];
            user_id = user.UserId;
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
            var roles = _userRepository.GetAllRoles();
            if (!String.IsNullOrEmpty(search))
            {
                bool isNumber = Role.TryParse(search, out Role numericValue);
                if (isNumber)
                {
                    roles = roles.Where(r => r.Id == numericValue);
                }
                else
                {
                    roles = roles.Where(r => r.Role.Contains(search));
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
            var count = roles.Count();
            result.count = count;

            Func<dynamic, dynamic> orderingFunction = i =>
                             orderBy == "Id" ? i.Id :
                             orderBy == "Role" ? i.Role : "";

            if (orderTypeAsc)
            {
                var allroles = roles
                        .OrderBy(orderingFunction)
                        .Skip(offset)
                        .Take(final_item_per_page)
                        .ToList();
                if (allroles == null)
                {
                    return NotFound(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status404NotFound, "Unable to fetch roles.", result));
                }
                if (allroles.Count == 0)
                {
                    result.rows = allroles;
                    return NotFound(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status404NotFound, "No Roles found.", result));
                }
                result.rows = allroles;
                return Ok(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status200OK, "Roles data found.", result));
            }
            else
            {
                var allroles = roles
                       .OrderByDescending(orderingFunction)
                       .Skip(offset)
                       .Take(final_item_per_page)
                       .ToList();

                if (allroles == null)
                {
                    return NotFound(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status404NotFound, "Unable to fetch roles.", result));
                }
                if (allroles.Count == 0)
                {
                    result.rows = allroles;
                    return NotFound(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status404NotFound, "No Roles found.", result));
                }
                result.rows = allroles;
                return Ok(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status200OK, "Roles data found.", result));
            }
        }

        [Authorize]
        [HttpGet("GetRoleById/{id}")]
        public IActionResult GetRoletById(Role id, [FromQuery] Guid user_id)
        {
            var user = (Users)HttpContext.Items["User"];
            user_id = user.UserId;
            if (user.UserId != user_id)
            {
                return Unauthorized(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status401Unauthorized, "userId and jwt token do not match.", result));
            }
            var role = _userRepository.GetRoleById(id);

            if (role == null)
            {
                return NotFound(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status404NotFound, "Role not found.", result));
            }
            result.patient = role;
            return Ok(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status200OK, "Role found.", result));
        }

        [Authorize]
        [HttpPost("create-role")]
        public IActionResult CreateRole(Roles role, Guid user_id)
        {
            var user = (Users)HttpContext.Items["User"];
            user_id = user.UserId;
            if (user.UserId != user_id)
            {
                return Unauthorized(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status401Unauthorized, "userId and jwt token do not match.", result));
            }

            var isAlreadyExists = _userRepository.CheckRoleName(role);
            if (isAlreadyExists)
            {
                var errMsg = "Role \"" + role.Role + "\" is already taken";
                return BadRequest(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status400BadRequest, errMsg, result));
            }

            var user_data = _userRepository.CreateRole(role, user_id);
            if (user_data == null)
            {
                var errMsg = "Invalid Role";
                return BadRequest(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status400BadRequest, errMsg, result));
            }
            var msg = "Role is created";
            return Ok(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status200OK, msg, role));
        }

        //[Authorize]
        //[HttpPut("UpdatePatientDetails/{patientId}")]
        //public async Task<IActionResult> UpdatePatientDetails(PatientDataRecord model, [FromQuery] Guid user_id)
        //{
        //    var user = (Users)HttpContext.Items["User"];
        //    if (user.UserId != user_id)
        //    {
        //        return Unauthorized(commonhelper.GenerateApiResponse(requestToken, 401, "userId and jwt token do not match.", result));
        //    }

        //    if (model.connection_id != 0 || !string.IsNullOrEmpty(model.first_name) || model.dob != null || !string.IsNullOrEmpty(model.gender) || !string.IsNullOrEmpty(model.patient_Identifier))
        //    {
        //        bool IsPatientIdentifierExit = _patientService.CheckDuplicatePatientIdentifier(model);
        //        if (IsPatientIdentifierExit)
        //        {
        //            var errmsg = "Duplicate Patient Identifier";
        //            return Conflict(commonhelper.GenerateApiResponse(requestToken, 409, errmsg, result));
        //        }

        //        bool IsAlreadyexist = _patientService.CheckDuplicatePatientData(model);
        //        if (IsAlreadyexist)
        //        {
        //            var errmsg = "Duplicate Data";
        //            return Conflict(commonhelper.GenerateApiResponse(requestToken, 409, errmsg, result));
        //        }

        //        _context.patients.Update(model);
        //        await _context.SaveChangesAsync();
        //        var msg = "Patient record updated successfully.";
        //        return Ok(commonhelper.GenerateApiResponse(requestToken, 200, msg, result));
        //    }
        //    else
        //    {
        //        var errmsg = "connection_id, first_name, last_name, dob, gender all are required fields.";
        //        return BadRequest(commonhelper.GenerateApiResponse(requestToken, 400, errmsg, result));
        //    }
        //}

        [HttpPut("delete-role/{id}")]
        public IActionResult DeleteRoleDetails(Role id, Guid user_id)
        {
            var user = (Users)HttpContext.Items["User"];
            user_id = user.UserId;
            if (user.UserId != user_id)
            {
                return Unauthorized(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status401Unauthorized, "userId and jwt token do not match.", result));
            }
            var role = _userRepository.GetRoleById(id);

            if (role == null)
            {
                return NotFound(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status404NotFound, "Patient data not found.", result));
            }
            var data = _userRepository.DeleteRole(role);
            if (data.Status)
            {
                var errmsg = "Something went wrong while delete the role.";
                return BadRequest(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status400BadRequest, errmsg, result));
            }
            var msg = "Patient record Deleted successfully.";
            return Ok(commonhelper.GenerateApiResponse(requestToken, StatusCodes.Status200OK, msg, result));
        }
    }
}
