using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class UserController : ControllerBase
    {
        CommonHelper commonhelper = new CommonHelper();
        private IUserRepository _userRepository;
        private IMapper _mapper;

        dynamic result = new ExpandoObject();
        string requestToken = "";
        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("get-all-users")]
        public IActionResult GetUsers([FromQuery] Guid user_id, [FromQuery] string search, [FromQuery] string page, [FromQuery] string per_page, [FromQuery] string sort_by, [FromQuery] string sort_type)
        {
            var user = (Users)HttpContext.Items["User"];
            if (user.UserId != user_id)
            {
                return Unauthorized(commonhelper.GenerateApiResponse(requestToken, 401, "userId and jwt token do not match.", result));
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
            var users = _userRepository.GetAllUsers(user_id);
            if (!String.IsNullOrEmpty(search))
            {
                bool isNumber = Guid.TryParse(search, out Guid numericValue);
                if (isNumber)
                {
                    users = users.Where(r => r.UserId == numericValue);
                }
                else
                {
                    users = users.Where(r => r.UserName.Contains(search));
                }
            }
            var orderBy = "UserId";
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
            var count = users.Count();
            result.count = count;

            Func<dynamic, dynamic> orderingFunction = i =>
                             orderBy == "UserId" ? i.UserId :
                             orderBy == "UserName" ? i.UserName :
                             orderBy == "Email" ? i.Email : "";

            if (orderTypeAsc)
            {
                var allusers = users
                        .OrderBy(orderingFunction)
                        .Skip(offset)
                        .Take(final_item_per_page)
                        .ToList();
                if (allusers == null)
                {
                    return NotFound(commonhelper.GenerateApiResponse(requestToken, 404, "Unable to fetch users.", result));
                }
                if (allusers.Count == 0)
                {
                    result.rows = allusers;
                    return Ok(commonhelper.GenerateApiResponse(requestToken, 200, "No users found.", result));
                }
                result.rows = allusers;
                return Ok(commonhelper.GenerateApiResponse(requestToken, 200, "Users data found.", result));
            }
            else
            {
                var allusers = users
                       .OrderByDescending(orderingFunction)
                       .Skip(offset)
                       .Take(final_item_per_page)
                       .ToList();

                if (allusers == null)
                {
                    return NotFound(commonhelper.GenerateApiResponse(requestToken, 404, "Unable to fetch Users.", result));
                }
                if (allusers.Count == 0)
                {
                    result.rows = allusers;
                    return Ok(commonhelper.GenerateApiResponse(requestToken, 200, "No Users found.", result));
                }
                result.rows = allusers;
                return Ok(commonhelper.GenerateApiResponse(requestToken, 200, "Users data found.", result));
            }
        }
    }
}
