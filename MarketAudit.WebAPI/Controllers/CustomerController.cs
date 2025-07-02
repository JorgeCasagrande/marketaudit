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

    public class CustomerController : BaseController
    {
        public ICustomerService service;

        public CustomerController(ICustomerService service, ILoggerManager logger)
        {
            this.logger = logger;
            this.service = service;
        }

        /// <summary>
        /// Return all Customers ordered by Name
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetCustomers(string states)
        {
            var customers = service.GetCustomers(states);
            return Ok(customers);
        }

        /// <summary>
        /// Return all Customer State
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
        /// Update selected customers states
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
        /// Delete selected customers states
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
        /// Save new or selected customers
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Save(Customer model)
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
        /// return form for new customer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetNewCustomer()
        {

            try
            {

                var data = service.GetNewCustomer();
                return MakeOkResponse(data.Message, data.Status, data.Data);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        /// <summary>
        /// return form for new customer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetCustomer(long id)
        {

            try
            {

                var data = service.GetCustomer(id);
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