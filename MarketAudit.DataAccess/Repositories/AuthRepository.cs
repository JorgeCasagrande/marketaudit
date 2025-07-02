using Dapper;
using Marketaudit.DataAccess.Interfaces;
using Marketaudit.DataAccess.Repositories;
using MarketAudit.Common.GlobalVariables;
using MarketAudit.Entities.Models;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace MarketAudit.DataAccess.Repositories
{
    public class AuthRepository : DataBaseRepository, IAuthRepository
    {
        public AuthRepository()
        {
            TABLE_NAME = "USER";
        }

        public AuthUser AuthUser2(string userName, string password)
        {
            AuthUser entity = new AuthUser();

            string query = string.Format("SELECT CASE WHEN EXISTS ( " +
                "SELECT * " +
                "FROM[{0}] " +
                "WHERE UserName = '{1}' and Password = '{2}') " +
                "THEN CAST(1 AS BIT) " +
                "ELSE CAST(0 AS BIT) END as AUTH, " +
                "CASE WHEN EXISTS( " +
                "SELECT * " +
                "FROM[{0}] " +
                "WHERE UserName = '{1}' and Password = '{2}') " +
                "THEN(SELECT Id FROM[{0}] WHERE UserName = '{1}') " +
                "ELSE 0 END as USERID", TABLE_NAME, userName , GenerateSHA256String(password));

            var result = ExecuteQuery(query);

            DataRow row = result[0];

            entity.Auth = ToBoolean(row["AUTH"]);
            entity.UserId = ToLong(row["USERID"]);

            return entity;

        }

        public AuthUser AuthUser(string userName, string password)
        {
            AuthUser result = new AuthUser();

            string query = string.Format(
                "SELECT Id as UserId, Enabled as IsEnabled " +
                "FROM[{0}] " +
                "WHERE UserName = '{1}' and Password = '{2}' ",
                TABLE_NAME, userName, GenerateSHA256String(password));

            IDbConnection conn = new SqlConnection(GlobalVariables.GetDatabaseConnectionString());
            conn.Open();

            UserModel entity;

            using (conn)
            {
                entity = conn.QueryFirstOrDefault<UserModel>(query);
                conn.Close();
            }

            if(entity != null)
            {
                result.Auth = entity.IsEnabled;
                result.UserId = entity.UserId;
                result.Message = !entity.IsEnabled ? "El usuario no esta habilitado" : string.Empty;
            }
            else
            {
                result.Auth = false;
                result.Message = "Los datos del usuario son incorrectos";
            }

            return result;

        }

        private string GenerateSHA256String(string inputString)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha256.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }

        private string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
    }
}
