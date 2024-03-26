using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using school_management_backend.Helpers;
using school_management_backend.Interfaces;
using school_management_backend.Models;
using System;
using System.Dynamic;

namespace school_management_backend.Controllers
{
    [Route("/admin/")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        CommonHelper commonhelper = new CommonHelper();
        private IDashboardRepository _dashboardRepository;
        private IMapper _mapper;

        dynamic result = new ExpandoObject();
        string requestToken = "";
        public DashboardController(IDashboardRepository dashboardRepository, IMapper mapper)
        {
            _dashboardRepository = dashboardRepository;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("count-admins")]
        public IActionResult TotalCountOfAdmins(Guid user_id) 
       {
            var user = (Users)HttpContext.Items["User"];
            if (user.UserId != user_id)
            {
                return Unauthorized(commonhelper.GenerateApiResponse(requestToken, 401, "userId and jwt token do not match.", result));
            }
            var numbers = _dashboardRepository.ReturnCount(user);
            return Ok(commonhelper.GenerateApiResponse(requestToken, 200, "Users data found.", numbers));
        }
    }
}
