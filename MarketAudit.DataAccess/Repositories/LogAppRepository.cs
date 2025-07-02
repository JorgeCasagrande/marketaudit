using Marketaudit.DataAccess.Repositories;
using MarketAudit.DataAccess.Interfaces;
using MarketAudit.Entities.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MarketAudit.DataAccess.Repositories
{
    public class LogAppRepository : DataBaseRepository, ILogAppRepository
    {
        public IEnumerable<LogApp> GetByDate(string date)
        {

            string fileName = Path.Combine(@"./LogsMk", formateDatePath(date) + "_logfile.log");
            List<string> fileLines = ReadFileLines(fileName);
            List<LogApp> logs = new List<LogApp>();

            try
            {
                foreach (var line in fileLines)
                {
                    if ((!line.Contains("Parameter name: length") && !line.Contains("The statement has been terminated") && !line.Contains("UpdateDetail")))
                        logs.Add(MapEntity(line));
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
            return logs;
        }

        public void InsertLogAppMk(LogAppMk model, ITransactionalContext transaction)
        {
            string query = string.Format("INSERT INTO [Log_App] ([UserId],[ProjectId],[PdvId],[Message],[Creation]) " +
                " VALUES ({0},{1},{2},'{3}',(SELECT GETDATE()))", (model.UserId == null ? "null" : model.UserId.ToString()), (model.ProjectId == null ? "null" : model.ProjectId.ToString()), (model.PdvId == null ? "null" : model.PdvId.ToString()), (model.Message == null ? "null" : model.Message));

            ExecuteQuery(query, transaction);
        }

        private LogApp MapEntity(string row)
        {
            string timeString = row.Substring(0, 24);
            int levelSpaceIndex = row.Substring(25).IndexOf(' ');
            string level = row.Substring(25, levelSpaceIndex);
            string description = row.Substring(25 + levelSpaceIndex + 1);
            description = description.Replace("\"", "\\\"");
            
            var entity = new LogApp
            {
                Time = DateTime.Parse(timeString).ToString("dd/MM/yyyy HH:mm:ss"),
                Level = level,
                Description = description
            };

            return entity;
        }

        private string formateDatePath(string date)
        {
            var dateArray = date.Split('-');

            return dateArray[0].PadLeft(4, '0') + "-" + dateArray[1].PadLeft(2, '0') + "-" + dateArray[2].PadLeft(2, '0');
        }
    }
}