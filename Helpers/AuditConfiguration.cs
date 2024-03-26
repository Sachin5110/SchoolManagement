using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Audit.WebApi;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using school_management_backend.Models;

namespace school_management_backend.Helpers
{
    public class AuditConfiguration
    {
        public static void AddAudit(MvcOptions mvcOptions)
        {
            mvcOptions.AddAuditFilter(config => config
                .LogRequestIf(r => r.Path.Value != "/admin/Audit/auditlog")
                .WithEventType("{verb} {controller}.{action}")
                .IncludeHeaders()
                .IncludeRequestBody()
                .IncludeResponseHeaders()
                .IncludeResponseBody()
            );
        }

        // Configure Audit Logs
        public static void ConfigureAudit(IServiceCollection serviceCollection, IConfiguration Configuration)
        {
            Audit.Core.Configuration.Setup()
                .UseDynamicAsyncProvider(config => config
                    .OnInsert(async ev => Console.WriteLine("")));

            Audit.Core.Configuration.AddCustomAction(Audit.Core.ActionType.OnEventSaving, scope =>
            {
                string AuditActionLog = scope.Event.GetWebApiAuditAction().ToJson();

                if (AuditActionLog == null)
                {
                    return;
                }

                string date = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                JObject auditlogData = JObject.Parse(AuditActionLog);
                Guid userId = Guid.Empty;
                string query_parameter = null;
                string requestToken = null;
                string userAgent = null;
                string httpMethod = null;
                string requestUrlPath = null;
                string ipAddress = null;
                string requestPayload = null;
                string responsePayload = null;
                int responseStatusCode = 0;

                if (auditlogData.ContainsKey("ActionParameters"))
                {
                    query_parameter = JsonConvert.SerializeObject(auditlogData["ActionParameters"]);
                    JObject actionParam = JObject.Parse(auditlogData["ActionParameters"].ToString());

                    if (actionParam.ContainsKey("user_id"))
                    {
                        if (actionParam["user_id"] != null)
                        {
                            userId = (Guid)actionParam["user_id"];
                        }
                    }
                }

                if (auditlogData.ContainsKey("Headers"))
                {
                    JObject headers = JObject.Parse(auditlogData["Headers"].ToString());

                    if (headers.ContainsKey("Authorization"))
                    {
                        requestToken = headers["Authorization"].ToString();
                    }

                    if (headers.ContainsKey("User-Agent"))
                    {
                        userAgent = headers["User-Agent"].ToString();
                    }
                }

                if (auditlogData.ContainsKey("HttpMethod"))
                {
                    httpMethod = auditlogData["HttpMethod"].ToString();
                }

                if (auditlogData.ContainsKey("RequestUrl"))
                {
                    string requestUrl = auditlogData["RequestUrl"].ToString();
                    Uri url = new Uri(requestUrl);
                    requestUrlPath = string.Format("{0}", url.AbsolutePath);
                }

                if (auditlogData.ContainsKey("IpAddress"))
                {
                    ipAddress = auditlogData["IpAddress"].ToString();
                }

                if (auditlogData.ContainsKey("ResponseStatusCode"))
                {
                    responseStatusCode = (int)auditlogData["ResponseStatusCode"];
                }

                if (auditlogData.ContainsKey("RequestBody"))
                {
                    requestPayload = JsonConvert.SerializeObject(auditlogData["RequestBody"].ToString());
                }

                if (auditlogData.ContainsKey("ResponseBody"))
                {
                    JObject responseBody = JObject.Parse(auditlogData["ResponseBody"].ToString());

                    if (responseBody.ContainsKey("Value"))
                    {
                        responsePayload = JsonConvert.SerializeObject(responseBody["Value"]);
                    }
                }

                auditlog audit = new auditlog();
                audit.auditlog_request_token = !string.IsNullOrEmpty(requestToken) ? requestToken.Substring(7) : requestToken;
                audit.auditlog_request_url = requestUrlPath;
                audit.auditlog_request_method = httpMethod;
                audit.auditlog_request_query = query_parameter;
                audit.auditlog_request_payload = requestPayload;
                audit.auditlog_response_payload = responsePayload;
                audit.auditlog_ip_address = ipAddress;
                audit.auditlog_browser_useragent = userAgent;
                audit.auditlog_event_actor = 1;
                audit.auditlog_event_actor_id = userId;
                audit.auditlog_authorized = 1;
                audit.auditlog_response_code = responseStatusCode;
                audit.SaveDetails(audit, date);
            });
        }
    }
}
