using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MarketAudit.Entities.Models.Generic;
using Microsoft.AspNetCore.Mvc;
using MarketAudit.Common.Log;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace MarketAudit.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : Controller
    {
        public ILoggerManager logger;
        protected IActionResult MakeOkResponse(string message,string status, object data = null)
        {
            ResponseData response = new ResponseData();
            response.Message = message;
            response.Status = status;
            response.Data = data;

            return Ok(response);
        }
        protected IActionResult InternalServerError(string message)
        {
            string formattedMessage = string.Format("{0}", message);
            return StatusCode(500, Jsonify("message", formattedMessage));
        }
        protected IActionResult Unauthorized(string message)
        {
            string formattedMessage = string.Format("{0}", message);
            return StatusCode(401, Jsonify("message", formattedMessage));
        }
        private string Jsonify(string key, string value)
        {
            string toSend = value.Replace("\\", "\\\\");
            toSend = toSend.Replace("\n", "\\n");
            toSend = toSend.Replace("\"", "\\\"");
            return "{\"" + key + "\": \"" + toSend + "\"}";
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            LogRequest(context);
        }

        private void LogRequest(ActionExecutingContext context)
        {
            if (this.ControllerContext.ActionDescriptor.ControllerName != "LogApp"
                && this.ControllerContext.ActionDescriptor.ControllerName != "Auth"
                && this.ControllerContext.ActionDescriptor.ControllerName != "AuthWeb")
            {
                object request = context.ActionArguments.Values.FirstOrDefault();
                string jsonString = JsonSerializer.Serialize(request);
                logger.LogInfo(string.Format("{3} - Controller: {0} - Action: {1} - Data: {2}",
                        this.ControllerContext.ActionDescriptor.ControllerName,
                        this.ControllerContext.ActionDescriptor.ActionName,
                        jsonString, FormatUserLog(context))
                        );
            }
        }

        private string FormatUserLog(ActionExecutingContext context)
        {
            string result = string.Empty;
            if (!context.HttpContext.Request.Headers.ContainsKey("userId") || !context.HttpContext.Request.Headers.ContainsKey("userName"))
            {
                result = "Swagger Request";
            }
            else if (string.IsNullOrEmpty(context.HttpContext.Request.Headers["userId"]) || string.IsNullOrEmpty(context.HttpContext.Request.Headers["userName"]))
            {
                result = "Swagger Request";
            }
            else
            {
                result = string.Format("UserId: {0} - UserName: {1}", context.HttpContext.Request.Headers["userId"], context.HttpContext.Request.Headers["userName"]);
            }

            return result;
        }
    }
}