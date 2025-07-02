using Marketaudit.Entities.Models.Response;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Generic;
using MarketAudit.Entities.Models.Request;
using MarketAudit.Entities.Models.Response;
using System.Collections.Generic;

namespace Marketaudit.Service.Interfaces
{
    public interface IProjectService
    {
        IEnumerable<Projects> GetProjectsByUserId(long userId);
        DataTableModel GetProjects(string states = null, string responsables = null);
        ResponseData GetStates();
        ResponseData Enable(long ids);
        ResponseData Save(Project model);
        ResponseData GetNewProject();
        ResponseData GetProject(long id);
        ResponseData SaveQuestions(long projectId, List<ImportQuestionModel> questions);
        ResponseData SavePdvs(long projectId, List<ImportPdvModel> pdvs);
        ResponseData GetProjectReports();
        DataTableModel GetQuestionByProjectId(long id);
        DataTableModel GetPdvByProjectId(long id);
        DataTableModel GetReportByProjectId(long projectId);
        List<PhotosReport> GetPhotoByProjectId(long projectId, string users, string pdvs, string routes, string questions);
        FilterPhotos GetFilterPhotoByProjectId(long projectId, string userId);
        ResponseData DeleteProject(long[] ids);
        DataTableModel GetProjectsDataTable(string states = null, string responsables = null);
        void CreateTableReportProcess();
        void CreateTableReportProcessByProjectId(long projectId);
        void ProcessReportTable(long projectId);
        DataTableModel GetReport(long projectId);
        bool ExistsTableReport(long projectId);
        void DeleteDuplicatePdvs(long projectId);
        List<ProjectQuestionTemplateModel> GetQuestionTemplateByProjectId(long projectId);
        List<ProjectPdvTemplateModel> GetPdvTemplateByProjectId(long projectId);
        void InsertSinglePdv(PdvModelRequest model);
        void SaveSingleQuestion(QuestionModelRequest model);
    }
}
