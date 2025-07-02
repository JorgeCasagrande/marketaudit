using System;
using System.Collections.Generic;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using System.Text.Json;
using System.Reflection;
using Microsoft.AspNetCore.Cors;

namespace Marketaudit.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class ProjectController : BaseController
    {
        public IProjectService service;
        public IUserService userService;
        private IHostingEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _contextAccessor;



        public ProjectController(IProjectService service,
            IUserService userService,
            IHostingEnvironment hostingEnvironment,
            IHttpContextAccessor contextAccessor,
            ILoggerManager logger)
        {
            this.logger = logger;
            this.service = service;
            this.userService = userService;
            _hostingEnvironment = hostingEnvironment;
            _contextAccessor = contextAccessor;
        }

        /// <summary>
        /// Return all Projects ordered by Name
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetProjects(string states, string responsables)
        {
            try
            {
                var projects = service.GetProjects(states, responsables);
                return Ok(projects);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        /// <summary>
        /// Return all Responsables
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetProjectReports()
        {
            try
            {
                var projects = service.GetProjectReports();
                return MakeOkResponse(projects.Message, projects.Status, projects.Data);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        /// <summary>
        /// Return questions by projectId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetQuestionByProjectId(long id)
        {
            var questions = service.GetQuestionByProjectId(id);
            return Ok(questions);
        }

        [HttpGet]
        public IActionResult GetQuestionTemplateByProjectId(long id)
        {
            var questions = service.GetQuestionTemplateByProjectId(id);
            return Ok(questions);
        }

        [HttpGet]
        public IActionResult GetPdvTemplateByProjectId(long id)
        {
            var questions = service.GetPdvTemplateByProjectId(id);
            return Ok(questions);
        }

        /// <summary>
        /// Return pdvs by projectId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetPdvByProjectId(long id)
        {
            var questions = service.GetPdvByProjectId(id);
            return Ok(questions);
        }

        /// <summary>
        /// Return all Responsables
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetResponsables()
        {
            try
            {
                var states = userService.GetResponsables();
                return MakeOkResponse(states.Message, states.Status, states.Data);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        /// <summary>
        /// Return all States
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
        /// Update selected Project
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Enable(long[] ids)
        {
            try
            {
                ResponseData enable = service.Enable(ids[0]);
                return MakeOkResponse(enable.Message, enable.Status, enable.Data);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        /// <summary>
        /// Save new or selected Project
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Save(Project model)
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
        /// return form for new project
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetNewProject()
        {

            try
            {

                var data = service.GetNewProject();
                return MakeOkResponse(data.Message, data.Status, data.Data);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        /// <summary>
        /// return form for new project
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetProject(long id)
        {

            try
            {

                var data = service.GetProject(id);
                return MakeOkResponse(data.Message, data.Status, data.Data);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult ImportQuestions()
        {
            try
            {
                string folderName = "Files";
                string webRootPath = _hostingEnvironment.ContentRootPath;
                string newPath = Path.Combine(webRootPath, folderName);

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                if (!long.TryParse(Request.Form["id"], out long projectId))
                {
                    projectId = 0;
                }

                if (Request.Form.Files.Count > 0)
                {
                    var postedFile = Request.Form.Files[0];
                    var fileName = postedFile.FileName;
                    var fileDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, "Files\\");
                    var filePath = fileDirectory + fileName;
                    if (!Directory.Exists(fileDirectory))
                    {
                        Directory.CreateDirectory(fileDirectory);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        postedFile.CopyTo(fileStream);
                    }

                    var questionExcel = QuestionExcel(filePath);

                    if (questionExcel.ContainError)
                        throw new Exception(questionExcel.ErrorMessage);

                    var result = service.SaveQuestions(projectId, questionExcel.List);

                    return MakeOkResponse(result.Message, result.Status, result.Data);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }


            return MakeOkResponse("", "", "");
        }

        private QuestionExcelModel QuestionExcel(string path)
        {
            QuestionExcelModel questionExcel = new QuestionExcelModel();
            questionExcel.List = new List<ImportQuestionModel>();

            IWorkbook wb = WorkbookFactory.Create(
                            new FileStream(Path.GetFullPath(path),
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.ReadWrite));

            ISheet ws = wb.GetSheetAt(0);

            for (int row = 1; row <= ws.LastRowNum; row++)
            {
                if (ws.GetRow(row).GetCell(1)!= null && !string.IsNullOrEmpty(ws.GetRow(row).GetCell(1).StringCellValue))
                {
                    var item = new ImportQuestionModel();

                    item.Orden = row;
                    //SetValueItemQuestionExcel(ws.GetRow(row).GetCell(0), row, Constantes.QuestionExcelFields.Orden, Constantes.OTExcelDataType.Numeric, ref item, ref questionExcel);
                    SetValueItemQuestionExcel(ws.GetRow(row).GetCell(1), row, Constantes.QuestionExcelFields.Pregunta, Constantes.OTExcelDataType.String, ref item, ref questionExcel);
                    SetValueItemQuestionExcel(ws.GetRow(row).GetCell(2), row, Constantes.QuestionExcelFields.Descripcion, Constantes.OTExcelDataType.String, ref item, ref questionExcel);
                    SetValueItemQuestionExcel(ws.GetRow(row).GetCell(3), row, Constantes.QuestionExcelFields.Miniatura, Constantes.OTExcelDataType.String, ref item, ref questionExcel);
                    SetValueItemQuestionExcel(ws.GetRow(row).GetCell(4), row, Constantes.QuestionExcelFields.Requerida, Constantes.OTExcelDataType.Bool, ref item, ref questionExcel);
                    SetValueItemQuestionExcel(ws.GetRow(row).GetCell(5), row, Constantes.QuestionExcelFields.Disparadora, Constantes.OTExcelDataType.String, ref item, ref questionExcel);
                    SetValueItemQuestionExcel(ws.GetRow(row).GetCell(6), row, Constantes.QuestionExcelFields.TipoPregunta, Constantes.OTExcelDataType.String, ref item, ref questionExcel);
                    SetValueItemQuestionExcel(ws.GetRow(row).GetCell(7), row, Constantes.QuestionExcelFields.TipoDato, Constantes.OTExcelDataType.String, ref item, ref questionExcel);
                    SetValueItemQuestionExcel(ws.GetRow(row).GetCell(8), row, Constantes.QuestionExcelFields.Respuestas, Constantes.OTExcelDataType.String, ref item, ref questionExcel);

                    item.Respuestas = ws.GetRow(row).GetCell(8) != null ? ws.GetRow(row).GetCell(8).StringCellValue : null;

                    if (item.Orden > 0 && item.Pregunta != null)
                        questionExcel.List.Add(item);
                }
            }

            wb.Close();

            var fileInfo = new FileInfo(path);
            fileInfo.Delete();

            return questionExcel;
        }

        [HttpPost]
        public IActionResult ImportPDV()
        {
            try
            {
                string folderName = "Files";
                string webRootPath = _hostingEnvironment.ContentRootPath;
                string newPath = Path.Combine(webRootPath, folderName);

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                if (!long.TryParse(Request.Form["id"], out long projectId))
                {
                    projectId = 0;
                }

                if (Request.Form.Files.Count > 0)
                {
                    var postedFile = Request.Form.Files[0];
                    var fileName = postedFile.FileName;
                    var fileDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, "Files\\");
                    var filePath = fileDirectory + fileName;
                    if (!Directory.Exists(fileDirectory))
                    {
                        Directory.CreateDirectory(fileDirectory);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        postedFile.CopyTo(fileStream);
                    }

                    var pdvExcel = PDVExcel(filePath);

                    if (pdvExcel.ContainError)
                        throw new Exception(pdvExcel.ErrorMessage);

                    var result = service.SavePdvs(projectId, pdvExcel.List);

                    return MakeOkResponse(result.Message, result.Status, result.Data);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }


            return MakeOkResponse("", "", "");
        }

        private PdvExcelModel PDVExcel(string path)
        {
            PdvExcelModel pdvExcel = new PdvExcelModel();
            pdvExcel.List = new List<ImportPdvModel>();

            IWorkbook wb = WorkbookFactory.Create(
                            new FileStream(Path.GetFullPath(path),
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.ReadWrite));

            ISheet ws = wb.GetSheetAt(0);

            for (int row = 1; row <= ws.LastRowNum; row++)
            {
                if (ws.GetRow(row).GetCell(2) != null && ws.GetRow(row).GetCell(2).ToString() != string.Empty)
                {
                    var item = new ImportPdvModel();

                    SetValueItemPdvExcel(ws.GetRow(row).GetCell(0), row, Constantes.PdvExcelFields.Censist, Constantes.OTExcelDataType.String, ref item, ref pdvExcel);
                    SetValueItemPdvExcel(ws.GetRow(row).GetCell(1), row, Constantes.PdvExcelFields.Number, Constantes.OTExcelDataType.Numeric, ref item, ref pdvExcel);
                    SetValueItemPdvExcel(ws.GetRow(row).GetCell(2), row, Constantes.PdvExcelFields.Name, Constantes.OTExcelDataType.String, ref item, ref pdvExcel);
                    SetValueItemPdvExcel(ws.GetRow(row).GetCell(3), row, Constantes.PdvExcelFields.Cuit, Constantes.OTExcelDataType.String, ref item, ref pdvExcel);
                    SetValueItemPdvExcel(ws.GetRow(row).GetCell(4), row, Constantes.PdvExcelFields.Address, Constantes.OTExcelDataType.String, ref item, ref pdvExcel);
                    SetValueItemPdvExcel(ws.GetRow(row).GetCell(5), row, Constantes.PdvExcelFields.PdvType, Constantes.OTExcelDataType.String, ref item, ref pdvExcel);
                    SetValueItemPdvExcel(ws.GetRow(row).GetCell(6), row, Constantes.PdvExcelFields.Route, Constantes.OTExcelDataType.String, ref item, ref pdvExcel);
                    SetValueItemPdvExcel(ws.GetRow(row).GetCell(7), row, Constantes.PdvExcelFields.Notes, Constantes.OTExcelDataType.String, ref item, ref pdvExcel);
                    SetValueItemPdvExcel(ws.GetRow(row).GetCell(8), row, Constantes.PdvExcelFields.Language, Constantes.OTExcelDataType.String, ref item, ref pdvExcel);

                    if (!string.IsNullOrEmpty(item.Censist))
                        pdvExcel.List.Add(item);
                }
            }

            wb.Close();

            var fileInfo = new FileInfo(path);
            fileInfo.Delete();

            return pdvExcel;
        }

        [HttpGet]
        public IActionResult GetReportByProjectId(long id)
        {
            DataTableModel report = new DataTableModel();
            if (service.ExistsTableReport(id))
                report = service.GetReport(id);
            else
                report = service.GetReportByProjectId(id);

            return Ok(report);
        }

        [HttpGet]
        public IActionResult GetPhotoByProjectId(long id, string users, string pdvs, string routes, string questions)
        {
            var report = service.GetPhotoByProjectId(id, users, pdvs, routes, questions);

            return Ok(report);
        }

        [HttpGet]
        public IActionResult GetDataFilterPhoto(long id, string userId)
        {
            var report = service.GetFilterPhotoByProjectId(id, userId);

            return Ok(report);
        }

        [HttpPost]
        public IActionResult DeleteProject(long[] ids)
        {
            try
            {
                ResponseData delete = service.DeleteProject(ids);

                return MakeOkResponse(delete.Message, delete.Status, delete.Data);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateTableReportProcess()
        {
            try
            {
                service.CreateTableReportProcess();

                return MakeOkResponse("Procceso Exitoso", "OK", null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateTableReportProcessById(long id)
        {
            try
            {
                service.CreateTableReportProcessByProjectId(id);

                return MakeOkResponse("Procceso Exitoso", "OK", null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult ProcessReportTable(long projectId)
        {
            try
            {
                service.ProcessReportTable(projectId);

                return MakeOkResponse("Procceso Exitoso", "OK", null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult GetReport(long projectId)
        {
            try
            {
                var data = service.GetReport(projectId);

                return MakeOkResponse("Procceso Exitoso", "OK", data);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult ExistsTableReport(long projectId)
        {
            try
            {
                var data = service.ExistsTableReport(projectId);

                return MakeOkResponse("Procceso Exitoso", "OK", data);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult DeleteDuplicatePdvs(long projectId)
        {
            try
            {
                service.DeleteDuplicatePdvs(projectId);

                return MakeOkResponse("Procceso Exitoso", "OK");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateQuestion([FromBody] QuestionModelRequest request)
        {
            try
            {
                var requestModel = request;

                return MakeOkResponse("Procceso Exitoso", "OK");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdatePdv([FromBody] PdvModelRequest request)
        {
            try
            {
                var requestModel = request;

                return MakeOkResponse("Procceso Exitoso", "OK");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult SaveQuestion([FromBody] QuestionModelRequest request)
        {
            try
            {
                service.SaveSingleQuestion(request);
                return MakeOkResponse("Procceso Exitoso", "OK");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult SavePdv([FromBody] PdvModelRequest request)
        {
            try
            {
                var requestModel = request;

                service.InsertSinglePdv(request);

                return MakeOkResponse("Procceso Exitoso", "OK");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        private void SetValueItemQuestionExcel(ICell cell, int row, string field, string fieldType ,ref ImportQuestionModel item, ref QuestionExcelModel oTExcel)
        {
            try
            {
                object value = new object();
                switch (fieldType)
                {
                    case Constantes.OTExcelDataType.String:
                        value = cell != null ? cell.StringCellValue : null;
                        break;
                    case Constantes.OTExcelDataType.Numeric:
                        value = cell != null ? cell.CellType == CellType.Numeric ? Convert.ToInt32(cell.NumericCellValue) : Convert.ToInt32(cell.StringCellValue) : 0;
                        break;
                    case Constantes.OTExcelDataType.Bool:
                        value = cell != null ? (cell.StringCellValue == "Si") : false;
                        break;
                }

                Type examType = typeof(ImportQuestionModel);
                PropertyInfo piInstance = examType.GetProperty(field);
                piInstance.SetValue(item, value);
            }
            catch (Exception ex)
            {
                oTExcel.ContainError = true;
                oTExcel.ErrorMessage += " - Fila: " + row + " - Columna: " + field + " - " + ex.Message + "|";
            }
        }

        private void SetValueItemPdvExcel(ICell cell, int row, string field, string fieldType, ref ImportPdvModel item, ref PdvExcelModel oTExcel)
        {
            try
            {
                object value = new object();
                switch (fieldType)
                {
                    case Constantes.OTExcelDataType.String:
                        value = cell != null ? cell.ToString() : null;
                        break;
                    case Constantes.OTExcelDataType.Numeric:
                        value = cell != null ? cell.CellType == CellType.Numeric ? Convert.ToInt64(cell.NumericCellValue) : Convert.ToInt32(cell.StringCellValue) : 0;
                        break;
                    case Constantes.OTExcelDataType.Bool:
                        value = cell != null ? (cell.StringCellValue == "Si") : false;
                        break;
                }

                Type examType = typeof(ImportPdvModel);
                PropertyInfo piInstance = examType.GetProperty(field);
                piInstance.SetValue(item, value);
            }
            catch (Exception ex)
            {
                oTExcel.ContainError = true;
                oTExcel.ErrorMessage += " - Fila: " + row + " - Columna: " + field + " - " + ex.Message + "|";
            }
        }
    }
}