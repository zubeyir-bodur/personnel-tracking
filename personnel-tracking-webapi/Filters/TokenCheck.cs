using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using personnel_tracking_webapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace personnel_tracking_webapi.Filters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TokenCheck : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Response.HasStarted)
            {
                string token = context.HttpContext.Request.Headers["Authorization"].ToString();

                if (string.IsNullOrEmpty(token)) context.Result = new JsonResult(new ResponseModel
                {
                    HasError = true,
                    ErrorMessage = "Token has not found!",
                    Data = null
                });

                if (!token.Contains("Bearer")) context.Result = new JsonResult(new ResponseModel
                {
                    HasError = true,
                    ErrorMessage = "Token is not valid!",
                    Data = null
                });
            }
        }
    }
}