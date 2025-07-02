using MarketAudit.Common.Log;
using MarketAudit.DataAccess.Interfaces;
using MarketAudit.DataAccess.Repositories;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Generic;
using MarketAudit.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MarketAudit.Service.Services
{
    public class LogAppService : ILogAppService
    {
        public ILogAppRepository repository;
        public LogAppService()
        {
            repository = new LogAppRepository();
        }

        public DataTableModel GetByDate(string date)
        {
            var entity = new DataTableModel();

            entity.columns = new string[] { "Hora", "Tipo", "Descripcion" };

            DataTable dt = new DataTable();

            foreach (var item in entity.columns)
            {
                dt.Columns.Add(item);
            }

            entity.data = repository.GetByDate(date).Select(x => new
            {
                x.Time,
                x.Level,
                x.Description
            }).ToArray();

            return entity;
        }

        public void InsertLogAppMk(LogAppMk model)
        {
            var logger = new LoggerManager();
            var transaction = new TransactionalContext();
            try
            {
                repository.InsertLogAppMk(model, transaction);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Message: {0}", ex.Message));
                transaction.Rollback();
            }
        }
    }
}
