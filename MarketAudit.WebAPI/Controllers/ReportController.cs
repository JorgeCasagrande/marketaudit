using System;
using System.IO;
using System.Linq;
using Marketaudit.Service.Interfaces;
using MarketAudit.Common.GlobalVariables;
using MarketAudit.Common.Log;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Request;
using MarketAudit.Entities.Models.Response;
using MarketAudit.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Marketaudit.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class ReportController : BaseController
    {
        public IReportService service;

        public ReportController(IReportService service, ILoggerManager logger)
        {
            this.logger = logger;
            this.service = service;
        }

        /// <summary>
        /// Servicio para enviar los datos de las encuestas
        /// </summary>
        [HttpPost]
        public IActionResult CreateReport([FromBody] InfoProject request)
        {
            try
            {
                var result = service.CreateReport(request);

                return MakeOkResponse(result.Message, result.Status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetReportPdv(long userId)
        {
            try
            {
                var data = service.GetReportPdv(userId);

                return MakeOkResponse("Reporte de Pdvs obtenido correctamente", "Ok", data);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }
    }
}