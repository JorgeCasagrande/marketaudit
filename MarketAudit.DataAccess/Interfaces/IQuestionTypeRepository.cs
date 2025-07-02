using Marketaudit.Entities.Models.Response;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketAudit.DataAccess.Interfaces
{
    public interface IQuestionTypeRepository
    {
        IEnumerable<QuestionType> GetAll();
        QuestionType Get(long id);
        QuestionType Get(string code,ITransactionalContext transaction);
    }
}
