using Marketaudit.Entities.Models.Response;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Request;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.DataAccess.Interfaces
{
    public interface IProjectRepository
    {
        IEnumerable<ProjectsModel> GetProjectsByUserId(long userId);
        IEnumerable<Project> GetProjects(string states);
        Project Get(long id);
        IEnumerable<ProjectGrid> GetProjectsReport();
        void Enable(long id, long state, ITransactionalContext transaction);
        void Delete(long id, ITransactionalContext transaction);
        void Update(Project model, ITransactionalContext transaction);
        void Insert(Project model, ITransactionalContext transaction);
        IEnumerable<ProjectGrid> GetProjectsByGrid(string states = null, string responsables = null);
        void InsertProjectQuestion(ProjectQuestion model, ITransactionalContext transaction);
        void FixDuplicateRoutes(long projectId, ITransactionalContext transactional);
        void CreateReportProjectTable(long projectId, List<ImportQuestionModel> questions);
        void CreateReportProjectTable(long projectId, List<QuestionModel> questions);
        void DropReportProjectTable(long projectId);
        bool ExistsReportProjectTable(long projectId, string usuario, string codigoPdv, string pdv, string ruta);
        void InsertReport(long projectId, string usuario, string codigoPdv, string pdv, string ruta, string fecha, string hora);
        void UpdateReport(long projectId, string usuario, string codigoPdv, string pdv, string ruta, string value, string order);
        IEnumerable<object> GetReport(long projectId);
        bool ExistsTableReport(long projectId);
        void DeleteDuplicatePdvs(long projectId, ITransactionalContext transaction);
    }
}
