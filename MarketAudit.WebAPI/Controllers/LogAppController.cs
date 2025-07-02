using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketAudit.Common.Log;
using MarketAudit.Entities.Models;
using MarketAudit.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketAudit.WebAPI.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class LogAppController : BaseController
    {

        public ILogAppService service;

        public LogAppController(ILogAppService service, ILoggerManager logger)
        {
            this.logger = logger;
            this.service = service;
        }

        [HttpGet]
        public IActionResult GetByDate(string date)
        {
            try
            {
                var data = service.GetByDate(date);
                return Ok(data);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult SetLogApp([FromBody] LogAppMk request)
        {
            try
            {
                service.InsertLogAppMk(request);
                return Ok();

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }
    }
}