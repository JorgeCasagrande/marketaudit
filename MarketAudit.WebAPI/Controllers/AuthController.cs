using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Marketaudit.Service.Interfaces;
using MarketAudit.Common.GlobalVariables;
using MarketAudit.Common.Log;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Request;
using MarketAudit.Entities.Models.Response;
using MarketAudit.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Marketaudit.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class AuthController : BaseController
    {
        public IAuthService service;
        public IProjectService projectService;
        public AuthController(IAuthService service, IProjectService projectService, ILoggerManager logger)
        {
            this.logger = logger;
            this.service = service;
            this.projectService = projectService;
        }

        /// <summary>
        /// Servicio para realizar el Login del usuario
        /// </summary>
        [HttpPost]
        public IActionResult Login([FromBody] Login_Request request)
        {
            try
            {
                AuthUser auth = service.AuthUser(request.User, request.Password);
                if (auth.Auth)
                {
                    var data = new Login();
                    data.UserId = auth.UserId;
                    data.Change = false;
                    data.UserName = request.User;
                    data.Projects = projectService.GetProjectsByUserId(auth.UserId).ToList();

                    return MakeOkResponse("Usuario Autorizado","Ok", data);
                }
                else
                    return MakeOkResponse(auth.Message, "Error");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }
    }
}