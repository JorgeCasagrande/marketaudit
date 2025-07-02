using Marketaudit.DataAccess.Interfaces;
using Marketaudit.DataAccess.Repositories;
using Marketaudit.Service.Interfaces;
using MarketAudit.Common.Log;
using MarketAudit.DataAccess.Interfaces;
using MarketAudit.DataAccess.Repositories;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Generic;
using MarketAudit.Entities.Models.Request;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;

namespace MarketAudit.Service.Services
{
    public class ReportService : IReportService
    {
        public IReportRepository repository;
        public IRouteRepository routeRepository;
        public IProjectRepository projectRepository;
        public ReportService()
        {
            repository = new ReportRepository();
            routeRepository = new RouteRepository();
            projectRepository = new ProjectRepository();
        }

        public ResponseData CreateReport(InfoProject model)
        {
            var result = new ResponseData();
            int countPdvError = 0;
            var logger = new LoggerManager();
            logger.LogInfo(string.Format("Creando Reporte - Proyecto: {0} - Censista: {1}", model.ProjectId, model.UserId));

            long questionId = 0;
            int orden = 0;

            foreach (var item in model.Pdvs)
            {
                var transaction = new TransactionalContext();
                try
                {
                    var routeId = routeRepository.GetRouteId(model.ProjectId, model.UserId, item.PdvId);
                    logger.LogInfo(string.Format("Pdv: {0} - Ruta: {1}", item.PdvId, routeId));

                    if(!repository.ExistReport(model.UserId, model.ProjectId, item.PdvId, routeId, transaction))
                    {
                        repository.Create(model.UserId, model.ProjectId, item.PdvId, routeId, transaction);
                        var reportMasterId = repository.GetReportMasterId(model.UserId, model.ProjectId, item.PdvId, routeId, transaction);
                        repository.CreateDetail(reportMasterId, model.ProjectId, transaction);

                        foreach (var item2 in item.Questions)
                        {
                            questionId = item2.QuestionId;
                            item2.Order = item2.Order != 0 ? item2.Order : repository.GetOrderQuestion(item2.QuestionId, transaction);
                            orden = item2.Order;
                            repository.UpdateDetail(reportMasterId, item2.QuestionId, item2.Order, item2.Value, transaction);
                        }

                        transaction.Commit();
                        logger.LogInfo(string.Format("Pdv Generado correctamente - Pdv: {0} - Ruta: {1}", item.PdvId, routeId));
                    }
                    else
                    {
                        var reportMasterId = repository.GetReportMasterId(model.UserId, model.ProjectId, item.PdvId, routeId, transaction);

                        foreach (var item2 in item.Questions)
                        {
                            questionId = item2.QuestionId;
                            item2.Order = item2.Order != 0 ? item2.Order : repository.GetOrderQuestion(item2.QuestionId, transaction);
                            orden = item2.Order;
                            repository.UpdateDetail(reportMasterId, item2.QuestionId, item2.Order, item2.Value, transaction);
                        }

                        transaction.Commit();
                        logger.LogInfo(string.Format("Reporte Actualizado - Proyecto: {0} - Censista: {1} - Pdv: {2} - Ruta: {3}", model.ProjectId, model.UserId, item.PdvId, routeId));
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(string.Format("ProjectId: {3} - UserId: {4} - Pdv: {0} - QuestionId: {1} - Orden: {2}", item.PdvId, questionId, orden, model.ProjectId, model.UserId));
                    logger.LogError(string.Format("Message: {0}", ex.Message));
                    countPdvError++;
                    transaction.Rollback();
                }
            }

            if (countPdvError > 0)
            {
                logger.LogError(string.Format("Reporte generado con errores - Proyecto: {0} - Censista: {1}", model.ProjectId, model.UserId));
            }
            else
            {
                logger.LogInfo(string.Format("Reporte generado correctamente - Proyecto: {0} - Censista: {1}", model.ProjectId, model.UserId));
            }

            result.Status = countPdvError > 0 ? "Error":"Ok";
            result.Message = countPdvError > 0 ? 
                string.Format("Hubo un error en la subida de PDVs, aun quedan {0} PDVs pendiente de envio. Por favor, vuelve a enviar los PDVs", countPdvError) : 
                string.Format("{0} PDVs pendiente de envio", countPdvError);

            return result;
        }

        public IEnumerable<ReportPdv> GetReportPdv(long userId)
        {
            var projects = projectRepository.GetProjectsByUserId(userId);
            var result =  new List<ReportPdv>();
            foreach(var item in projects)
            {
                var entity = repository.GetReportPdv(item.Id, userId);
                entity.ProjectId = item.Id;
                entity.Name = item.Name;

                result.Add(entity);
            }

            return result;
        }
    }
}

