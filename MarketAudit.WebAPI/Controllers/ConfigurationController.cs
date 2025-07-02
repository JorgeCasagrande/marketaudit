using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketAudit.Common.Log;
using MarketAudit.Entities.Models;
using MarketAudit.Service.Interfaces;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using MarketAudit.Entities.Models.Response;

namespace MarketAudit.WebAPI.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class ConfigurationController : BaseController
    {

        public IConfigurationService service;

        public ConfigurationController(IConfigurationService service, ILoggerManager logger)
        {
            this.logger = logger;
            this.service = service;
        }

        [HttpGet]
        public IActionResult GetConfiguration(string language)
        {
            try
            {
                var data = service.GetConfiguration(language);
                return Ok(data);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetConfigurationXML(string language)
        {
            try
            {
                var data = service.GetConfiguration(language);
                var resourcesXml = MapToResourcesXml(data);
                return Ok(SerializeToXml(resourcesXml));

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return InternalServerError(ex.Message);
            }
        }

        private ResourcesXml MapToResourcesXml(IEnumerable<Recursos> resources)
        {
            var resourcesXml = new ResourcesXml
            {
                Resources = new List<ResourceXml>()
            };

            foreach (var resource in resources)
            {
                resourcesXml.Resources.Add(new ResourceXml
                {
                    Name = resource.Name,
                    Value = resource.Value
                });
            }

            return resourcesXml;
        }

        private string SerializeToXml(ResourcesXml resourcesXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ResourcesXml));
            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, resourcesXml);
                return stringWriter.ToString();
            }
        }
    }
}