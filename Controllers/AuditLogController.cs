using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Dynamic;
using school_management_backend.Models;
using school_management_backend.Helpers;

namespace school_management_backend.Controllers
{

    [ApiController]
    [Route("/admin/Audit/")]
    public class auditlogController : ControllerBase
    {
        CommonHelper commonhelper = new CommonHelper();
        private readonly DBContext _context;
        dynamic result = new ExpandoObject();
        string requestToken = "";
        public auditlogController(DBContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("auditlog")]
        public IActionResult GetAllauditlog([FromQuery] Guid user_id, [FromQuery] string search, [FromQuery] string page, [FromQuery] string per_page)
        {
            var user = (Users)HttpContext.Items["User"];
            if (user.UserId != user_id)
            {
                return Unauthorized(commonhelper.GenerateApiResponse(requestToken, 401, "userId and jwt token do not match.", result));
            }

            int final_page = 1;
            if (!string.IsNullOrEmpty(page))
            {
                final_page = short.Parse(page);
            }

            int final_item_per_page = 10;
            if (!string.IsNullOrEmpty(per_page))
            {
                final_item_per_page = short.Parse(per_page);
            }
            int offset = 0;
            if (final_page > 1)
            {
                offset = (final_page - 1) * final_item_per_page;
            }

            var auditlogs = from f in _context.auditlog
                            select f;

            if (!string.IsNullOrEmpty(search))
            {
                bool isNumber = int.TryParse(search, out int numericValue);
                if (isNumber)
                {
                    auditlogs = auditlogs.Where(a => a.auditlog_id == numericValue
                                           || a.auditlog_response_code == numericValue);
                }
                else
                {
                    auditlogs = auditlogs.Where(a => a.auditlog_request_url.Contains(search)
                                           || a.auditlog_request_method.Contains(search) || a.auditlog_ip_address.Contains(search));
                }
            }

            var orderBy = "auditlog_id";
            if (!string.IsNullOrEmpty(HttpContext.Request.Query["sort_by"]))
            {
                orderBy = HttpContext.Request.Query["sort_by"];
            }

            bool orderTypeAsc = false;
            if (!string.IsNullOrEmpty(HttpContext.Request.Query["sort_type"]))
            {
                if (HttpContext.Request.Query["sort_type"] == "asc")
                {
                    orderTypeAsc = true;
                }
            }

            Func<dynamic, dynamic> orderingFunction = i =>
                              orderBy == "auditlog_id" ? i.auditlog_id :
                              orderBy == "auditlog_request_token" ? i.auditlog_request_token :
                              orderBy == "auditlog_request_url" ? i.auditlog_request_url :
                              orderBy == "auditlog_request_method" ? i.auditlog_request_method :
                              orderBy == "auditlog_request_query" ? i.auditlog_request_query :
                              orderBy == "auditlog_request_payload" ? i.auditlog_request_payload :
                              orderBy == "auditlog_response_payload" ? i.auditlog_response_payload :
                              orderBy == "auditlog_ip_address" ? i.auditlog_ip_address :
                              orderBy == "auditlog_browser_useragent" ? i.auditlog_browser_useragent :
                              orderBy == "auditlog_event_datetime" ? i.auditlog_event_datetime :
                              orderBy == "auditlog_event_actor" ? i.auditlog_event_actor :
                              orderBy == "auditlog_event_actor_id" ? i.auditlog_event_actor_id :
                              orderBy == "auditlog_authorized" ? i.auditlog_authorized :
                              orderBy == "auditlog_response_code" ? i.auditlog_response_code :
                              orderBy == "auditlog_created" ? i.auditlog_created : "";

            var count = _context.auditlog.Count();
            result.count = count;
            if (orderTypeAsc)
            {
                var allauditlogs = auditlogs
                        .OrderBy(orderingFunction)
                        .Skip(offset)
                        .Take(final_item_per_page)
                        .ToList();
                if (allauditlogs == null)
                {
                    return NotFound(commonhelper.GenerateApiResponse(requestToken, 404, "No data found..", result));
                }
                if (allauditlogs.Count == 0)
                {
                    result.rows = allauditlogs;
                    return Ok(commonhelper.GenerateApiResponse(requestToken, 200, "No Audit found.", result));
                }
                result.rows = allauditlogs;
                return Ok(commonhelper.GenerateApiResponse(requestToken, 200, "Audit log data found.", result));
            }
            else
            {
                var allfacilities = auditlogs
                       .OrderByDescending(orderingFunction)
                       .Skip(offset)
                       .Take(final_item_per_page)
                       .ToList();

                if (allfacilities == null)
                {
                    return NotFound(commonhelper.GenerateApiResponse(requestToken, 404, "Unable to fetch Audit.", result));
                }
                if (allfacilities.Count == 0)
                {
                    result.rows = allfacilities;
                    return Ok(commonhelper.GenerateApiResponse(requestToken, 200, "No Audit found.", result));
                }
                result.rows = allfacilities;
                return Ok(commonhelper.GenerateApiResponse(requestToken, 200, "Audit log data found.", result));
            }
        }

        [Authorize]
        [HttpGet("GetauditlogById/{audit_log_id}")]
        public IActionResult GetAllauditlog(int audit_log_id, [FromQuery] Guid user_id)
        {
            var user = (Users)HttpContext.Items["User"];
            if (user.UserId != user_id)
            {
                return Unauthorized(commonhelper.GenerateApiResponse(requestToken, 401, "userId and jwt token do not match.", result));
            }

            var auditlogData = _context.auditlog.Where(a => a.auditlog_id == audit_log_id).FirstOrDefault();
            if (auditlogData == null)
            {
                return NotFound(commonhelper.GenerateApiResponse(requestToken, 404, "Unable to fetch Logs.", result));
            }

            result.logData = auditlogData;
            return Ok(commonhelper.GenerateApiResponse(requestToken, 200, "Audit log found.", result));
        }
    }
}
