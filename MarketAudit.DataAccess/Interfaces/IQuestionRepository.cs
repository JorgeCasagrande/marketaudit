using Marketaudit.Entities.Models.Response;
using MarketAudit.DataAccess.Repositories;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.DataAccess.Interfaces
{
    public interface IQuestionRepository
    {
        IEnumerable<QuestionModel> GetQuestionByProjectId(long projectId);
        IEnumerable<QuestionModel> GetQuestionByUserId(long userId);
        long GetQuestions(long projectId);
        bool Exist(string question, ITransactionalContext transaction);
        Question Get(string pregunta, ITransactionalContext transaction);
        long Insert(Question model, ITransactionalContext transaction);
        void InsertQuestionResponse(QuestionResponse model, ITransactionalContext transaction);
        void InsertQuestionLogic(QuestionLogic model, ITransactionalContext transaction);
        IEnumerable<Filter> GetQuestionFilter(long projectId, TransactionalContext transaction);
        IEnumerable<ProjectQuestionTemplateModel> GetQuestionTemplateByProjectId(long projectId);
        void DeleteQuestionProject(long projectId, ITransactionalContext transaction);
    }
}
