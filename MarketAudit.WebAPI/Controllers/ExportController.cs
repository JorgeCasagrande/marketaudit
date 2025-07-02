using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using Marketaudit.Service.Interfaces;
using MarketAudit.Common.GlobalVariables;
using MarketAudit.Common.Log;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Generic;
using MarketAudit.Entities.Models.Request;
using MarketAudit.Entities.Models.Response;
using MarketAudit.WebAPI.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Marketaudit.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ExportController : BaseController
    {
        private ICustomerService _customerService;
        private IUserService _userService;
        private IProjectService _projectService;
        private IHostingEnvironment hostingEnvironment;

        public ExportController(ICustomerService customerService, IUserService userService, IProjectService projectService, ILoggerManager logger, IHostingEnvironment hostingEnvironment)
        {
            this.logger = logger;
            _customerService = customerService;
            _userService = userService;
            _projectService = projectService;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        [Produces("application/vnd.ms-excel")]
        public IActionResult GetReport(string report, long id = 0)
        {
            DataTableModel data;

            try
            {
                switch (report)
                {
                    case "customer":
                        data = _customerService.GetCustomers();
                        break;
                    case "user":
                        data = _userService.GetUsers();
                        break;
                    case "project":
                        data = _projectService.GetProjectsDataTable();
                        break;
                    case "questionProject":
                        data = _projectService.GetQuestionByProjectId(id);
                        break;
                    case "pdvProject":
                        data = _projectService.GetPdvByProjectId(id);
                        break;
                    case "informe-auditoria":
                        data = _projectService.GetReportByProjectId(id);
                        break;
                    default:
                        data = null;
                        break;
                }

                logger.LogInfo("Generate Report");
                var streamContent = GenerateReport(data);

                logger.LogInfo("Return Report");
                return File(streamContent.ToArray(), "application/vnd.ms-excel", "archivo.xlsx");
                
            }
            catch (Exception exception)
            {
                return InternalServerError(exception.Message);
            }
        }

        [HttpGet]
        [Produces("application/zip")]
        public IActionResult GetPhotos(long id, string users, string pdvs, string routes, string questions)
        {
            List<PhotosReport> data;

            try
            {
                logger.LogInfo("GetPhotos - id: " + id.ToString());
                data = _projectService.GetPhotoByProjectId(id, users, pdvs, routes, questions);

                logger.LogInfo("GetPhotos - dataCount:" + data?.Count);

                var streamContent = GenerateReportPhoto(data, id);
                logger.LogInfo("GetPhotos - Fin ");
                return File(streamContent.ToArray(), "application/zip", "archivo.zip");

            }
            catch (Exception exception)
            {
                logger.LogError(exception.Message);
                return InternalServerError(exception.Message);
            }
        }

        private MemoryStream GenerateReport(DataTableModel data)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            foreach (var item in data.columns)
            {
                dt.Columns.Add(item);
            }


            if (data.data.Length > 0)
            {
                Type myType = data.data[0].GetType();
                IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

                List<string> line = null;
                if (!data.data[0].GetType().IsArray)
                {
                    if (props.Count > 0)
                    {
                        foreach (var item in data.data)
                        {
                            var row = dt.NewRow();
                            var c = 0;
                            foreach (PropertyInfo prop in props)
                            {
                                object propValue = prop.GetValue(item, null);
                                row[c] = propValue;
                                c++;
                            }
                            dt.Rows.Add(row);
                        }
                    }
                    else
                    {
                        foreach (IDictionary<string, object> item in data.data)
                        {
                            var row = dt.NewRow();
                            var c = 0;
                            foreach (var prop in item.Keys)
                            {
                                row[c] = item[prop].ToString();
                                c++;
                            }

                            dt.Rows.Add(row);
                        }
                    }
                }
                else
                {
                    foreach (object[] objectItem in data.data)
                    {
                        var row = dt.NewRow();
                        var c = 0;
                        foreach (object item in objectItem)
                        {
                            if (item.ToString().Contains("https://weask-images.s3.amazonaws.com"))
                                row[c] = item.ToString().Replace("|", Environment.NewLine);
                            else
                                row[c] = item.ToString();

                            c++;
                        }
                        dt.Rows.Add(row);
                    }
                }
            }


            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Datos");
            worksheet.Cell("A1").InsertTable(dt);
            var path = Path.Combine(hostingEnvironment.ContentRootPath, "Files\\" + Guid.NewGuid() + "_download.xlsx");
            workbook.SaveAs(path);

            var byteArray = System.IO.File.ReadAllBytes(path);
            var stream = new MemoryStream(byteArray);
            var streamWriter = new StreamWriter(stream);
            System.IO.File.Delete(path);
            streamWriter.Flush();
            stream.Position = 0;

            return stream;
        }

        private MemoryStream GenerateReportPhoto(List<PhotosReport> data, long id)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            var path = Path.Combine(hostingEnvironment.ContentRootPath, "Files\\" + Guid.NewGuid() + "_download.zip");

            var pathCopy = Path.Combine(hostingEnvironment.ContentRootPath, "Files\\" + id + "_downloadPhoto");

            if(!Directory.Exists(pathCopy))
            {
                Directory.CreateDirectory(pathCopy);
            }
            
            using (var streamZip = System.IO.File.OpenWrite(path))
            {
                using (ZipArchive archive = new ZipArchive(streamZip, System.IO.Compression.ZipArchiveMode.Create))
                {
                    var c = 1;
                    foreach (var item in data)
                    {
                        if (!System.IO.File.Exists(pathCopy + "\\" + item.Img.Replace("https://weask-images.s3.amazonaws.com", "")))
                        {
                            WebClient webClient = new WebClient();
                            webClient.DownloadFile(item.Img, pathCopy + "\\" + item.Img.Replace("https://weask-images.s3.amazonaws.com", ""));
                        }
                        //archive.CreateEntryFromFile(pathCopy + "\\" + item.Img.Replace("https://weask-images.s3.amazonaws.com", ""), item.Author.ToUpper() + "\\" + item.Title.ToUpper() + "\\" + item.Img.Replace("https://weask-images.s3.amazonaws.com/", ""));
                        archive.CreateEntryFromFile(pathCopy + "\\" + item.Img.Replace("https://weask-images.s3.amazonaws.com", ""), c + "_" + item.Author.ToUpper() + "_" + item.Question.Replace(" - ", "_").Replace("|", "_") + "_" + item.Title.ToUpper().Replace(" - ", "_").Replace(" - ", "_").Replace("|", "_") + ".jpg");
                        c++;
                    }
                }
            }

            //if (Directory.Exists(pathCopy))
            //{
            //    Directory.Delete(pathCopy);
            //}

            var byteArray = System.IO.File.ReadAllBytes(path);
            var stream = new MemoryStream(byteArray);
            var streamWriter = new StreamWriter(stream);
            //System.IO.File.Delete(path);
            streamWriter.Flush();
            stream.Position = 0;

            return stream;
        }
    }
}