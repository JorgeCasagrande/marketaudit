using MarketAudit.Entities;

namespace Marketaudit.Entities.Models.Response
{
    public class Responses : Entity
    {
        public string Response { get; set; }
        public long QuestionId { get; set; }
        public string IconResponse { get; set; }
        public long ValueLogic { get; set; }
    }
}
