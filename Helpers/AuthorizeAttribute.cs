using System;
using System.Dynamic;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using school_management_backend.Models;
using school_management_backend.Enum;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    dynamic result = new ExpandoObject();
    string requestToken = "";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = (Users)context.HttpContext.Items["User"];
       
        if (user == null || user.Token.Status == (TokenStatus.Expired).ToString())
        {
            context.Result = new JsonResult(new { requestToken, message = "Unauthorized", error = "", code = "401", result }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class TeacherAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    dynamic result = new ExpandoObject();
    string requestToken = "";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = (Teacher)context.HttpContext.Items["Teacher"];

        if (user == null || user.Token.Status == (TokenStatus.Expired).ToString())
        {
            context.Result = new JsonResult(new { requestToken, message = "Unauthorized", error = "", code = "401", result }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class StudentAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    dynamic result = new ExpandoObject();
    string requestToken = "";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = (Student)context.HttpContext.Items["Student"];

        if (user == null || user.Token.Status == (TokenStatus.Expired).ToString())
        {
            context.Result = new JsonResult(new { requestToken, message = "Unauthorized", error = "", code = "401", result }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}

