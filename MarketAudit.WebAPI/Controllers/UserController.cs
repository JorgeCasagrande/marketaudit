using System;
using System.IO;
using System.Linq;
using Marketaudit.Service.Interfaces;
using MarketAudit.Common.GlobalVariables;
using MarketAudit.Common.Log;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Generic;
using MarketAudit.Entities.Models.Request;
using MarketAudit.Entities.Models.Response;
using MarketAudit.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Marketaudit.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class UserController : BaseController
    {
        public IUserService service;

        public UserController(IUserService service, ILoggerManager logger)
        {
            this.logger = logger;
            this.service = service;
        }

        /// <summary>
        /// Return all Users ordered by Name
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetUsers(string roles, string states)
        {
            var customers = service.GetUsers(roles, states);
            return Ok(customers);
        }

        /// <summary>
        /// Return all User Rol
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetRoles()
        {
            try
            {
                var states = service.GetRoles();
                return MakeOkResponse(states.Message, states.Status, states.Data);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        /// <summary>
        /// Return all User State
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetStates()
        {
            try
            {
                var states = service.GetStates();
                return MakeOkResponse(states.Message, states.Status, states.Data);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        /// <summary>
        /// Update selected Users 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Enable(long[] ids)
        {
            try
            {

                ResponseData enable = service.Enable(ids);

                return MakeOkResponse(enable.Message, enable.Status, enable.Data);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        /// <summary>
        /// Delete selected Users
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete(long[] ids)
        {
            try
            {
                ResponseData delete = service.Delete(ids);

                return MakeOkResponse(delete.Message, delete.Status, delete.Data);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        /// <summary>
        /// Save new or selected Users
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Save(User model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ResponseData save = service.Save(model);
                    return MakeOkResponse(save.Message, save.Status, save.Data);
                }
                else
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return MakeOkResponse(message, "Error");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        /// <summary>
        /// return form for new User
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetNewUser()
        {
            try
            {
                var data = service.GetNewUser();
                return MakeOkResponse(data.Message, data.Status, data.Data);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        /// <summary>
        /// return form for new User
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUser(long id)
        {
            try
            {
                var data = service.GetUser(id);
                return MakeOkResponse(data.Message, data.Status, data.Data);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

       

    }
}