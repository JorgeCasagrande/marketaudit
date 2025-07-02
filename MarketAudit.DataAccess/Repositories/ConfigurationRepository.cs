using Marketaudit.DataAccess.Repositories;
using MarketAudit.Common.GlobalVariables;
using MarketAudit.DataAccess.Interfaces;
using MarketAudit.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.IO;
using System.Text;
using System.Linq;

namespace MarketAudit.DataAccess.Repositories
{
    public class ConfigurationRepository : DataBaseRepository, IConfigurationRepository
    {
        public List<Recursos> GetConfiguration(string languaje)
        {
            string query = string.Format("Select Clave as Name, Valor as Value from Recursos where Idioma = '{0}' ", languaje);

            IDbConnection conn = new SqlConnection(GlobalVariables.GetDatabaseConnectionString());
            conn.Open();
            List<Recursos> entities;

            using (conn)
            {
                entities = conn.Query<Recursos>(query).ToList();
                conn.Close();
            }

            return entities;
        }
    }
}