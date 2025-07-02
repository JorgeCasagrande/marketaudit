using Marketaudit.Entities.Models.Response;
using MarketAudit.DataAccess.Interfaces;
using MarketAudit.DataAccess.Repositories;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Marketaudit.DataAccess.Repositories
{
    public class UserRepository : DataBaseRepository, IUserRepository
    {
        public UserRepository()
        {
            TABLE_NAME = "USER";
        }

        public void Delete(long id, ITransactionalContext transaction)
        {
            string query = string.Format("Delete [{0}] WHERE Id = {1}", TABLE_NAME, id);

            ExecuteQuery(query, transaction);
        }

        public void Enable(User user, ITransactionalContext transaction)
        {
            string query = string.Format("UPDATE [{0}] SET [Enabled] = '{1}',[EndDate] = '{2}' WHERE Id = {3}", TABLE_NAME, user.Enabled ? 1 : 0, !user.EndDate.HasValue ? DBNull.Value : (object)user.EndDate, user.Id);

            ExecuteQuery(query, transaction);
        }

        public User Get(long id)
        {
            string whereClause = string.Format("Where c.Id ={0}", id);
            string query = string.Format("SELECT c.Id, c.UserName, c.Password, c.Name, c.LastName, c.Email, c.RoleId, c.Enabled, c.Image, c.Creation, c.IsUserTest, c.EndDate FROM [{0}] c {1} ", TABLE_NAME, whereClause);

            var result = ExecuteQuery(query);

            var row = result[0];

            return new User
            {
                Id = ToLong(row["Id"]),
                UserName = ToString(row["UserName"]),
                Password = ToString(row["Password"]),
                Name = ToString(row["Name"]),
                LastName = ToString(row["LastName"]),
                Email = ToString(row["Email"]),
                RoleId = ToLong(row["RoleId"]),
                Image = ToString(row["Image"]),
                Enabled = Convert.ToBoolean(ToString(row["Enabled"])),
                Creation = Convert.ToDateTime(row["Creation"]),
                EndDate = !string.IsNullOrEmpty(ToString(row["EndDate"])) ? DateTime.Parse(ToString(row["EndDate"])) : (DateTime?)null,
                UserTest = ToBoolean(row["IsUserTest"])
            };
        }

        public User Get(string userName, ITransactionalContext transaction)
        {
            string whereClause = string.Format("Where c.UserName ='{0}'", userName);
            string query = string.Format("SELECT c.Id, c.UserName, c.Password, c.Name, c.LastName, c.Email, c.RoleId, c.Enabled, c.Image, c.Creation, c.IsUserTest, c.EndDate FROM [{0}] c {1} ", TABLE_NAME, whereClause);

            var result = ExecuteQuery(query,transaction);

            if (result.Count == 0)
                return null;

            var row = result[0];

            return new User
            {
                Id = ToLong(row["Id"]),
                UserName = ToString(row["UserName"]),
                Password = ToString(row["Password"]),
                Name = ToString(row["Name"]),
                LastName = ToString(row["LastName"]),
                Email = ToString(row["Email"]),
                RoleId = ToLong(row["RoleId"]),
                Image = ToString(row["Image"]),
                Enabled = Convert.ToBoolean(ToString(row["Enabled"])),
                Creation = Convert.ToDateTime(row["Creation"]),
                EndDate = !string.IsNullOrEmpty(ToString(row["EndDate"])) ? DateTime.Parse(ToString(row["EndDate"])) : (DateTime?)null,
                UserTest = ToBoolean(row["IsUserTest"])
            };
        }

        public IEnumerable<User> GetUsers(string roles, string states)
        {
            string whereClause = "Where 1=1 ";

            if (!string.IsNullOrEmpty(roles) && roles.Length > 0)
            {
                whereClause += string.Format(" and c.RoleId in ({0})", roles);
            }

            if (!string.IsNullOrEmpty(states) && states.Length > 0)
            {
                whereClause += string.Format(" and c.Enabled in ({0})", states); 
            }

            string query = string.Format("SELECT c.Id, c.UserName, c.Password, c.Name, c.LastName, c.Email, c.RoleId, c.Enabled, c.Image, c.Creation, c.IsUserTest ,c.EndDate  FROM [{0}] c {1} Order by c.Name asc ", TABLE_NAME, whereClause);

            var result = ExecuteQuery(query);

            List<User> entities = new List<User>();

            foreach (DataRow row in result)
            {

                entities.Add(new User()
                {
                    Id = ToLong(row["Id"]),
                    UserName = ToString(row["UserName"]),
                    Password = ToString(row["Password"]),
                    Name = ToString(row["Name"]),
                    LastName = ToString(row["LastName"]),
                    Email = ToString(row["Email"]),
                    RoleId = ToLong(row["RoleId"]),
                    Image = ToString(row["Image"]),
                    Creation = Convert.ToDateTime(row["Creation"]),
                    EndDate = !string.IsNullOrEmpty(ToString(row["EndDate"])) && !ToString(row["EndDate"]).Contains("1/1/1900") ? DateTime.Parse(ToString(row["EndDate"])) : (DateTime?)null,
                    Enabled = Convert.ToBoolean(ToString(row["Enabled"])),
                    UserTest = ToBoolean(row["IsUserTest"])
                });
            }

            return entities;
        }

        public void Update(User model, ITransactionalContext transaction)
        {
            string query = string.Format("UPDATE [{0}] SET [UserName] = '{1}', " +
                "[Password] = '{2}', " +
                "[Name] = '{3}', " +
                "[LastName] = '{4}', " +
                "[Email] = '{5}', " +
                "[RoleId] = '{6}', " +
                "[Image] = '{7}', " +
                "[Enabled] = '{8}', " +
                "[IsUserTest] = {9} " +
                "WHERE Id = {10}", TABLE_NAME, RemoveCharacterInvalid(model.UserName), RemoveCharacterInvalid(model.Password), RemoveCharacterInvalid(model.Name), RemoveCharacterInvalid(model.LastName), RemoveCharacterInvalid(model.Email), model.RoleId, model.Image, model.Enabled, model.UserTest ? "1" : "0", model.Id);

            ExecuteQuery(query, transaction);
        }

        public void Insert(User model, ITransactionalContext transaction)
        {
            string query = string.Format("INSERT INTO [{0}]  ([UserName], [Password], [Name], [LastName], [Email], [RoleId], [Image], [Enabled], [Creation], [IsUserTest]) " +
                " VALUES ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',{10})", 
                TABLE_NAME, RemoveCharacterInvalid(model.UserName), RemoveCharacterInvalid(model.Password), RemoveCharacterInvalid(model.Name), RemoveCharacterInvalid(model.LastName), RemoveCharacterInvalid(model.Email), model.RoleId, model.Image, model.Enabled, model.Creation, model.UserTest ? "1":"0");

            ExecuteQuery(query, transaction);
        }

        public IEnumerable<User> GetResponsables()
        {
            string whereClause = "Where c.RoleId = 4 and c.Enabled = 1";

            string query = string.Format("SELECT c.Id, c.UserName, c.Password, c.Name, c.LastName, c.Email, c.RoleId, c.Enabled, c.Image, c.Creation, c.EndDate  FROM [{0}] c {1} Order by c.Name asc ", TABLE_NAME, whereClause);

            var result = ExecuteQuery(query);

            List<User> entities = new List<User>();

            foreach (DataRow row in result)
            {

                entities.Add(new User()
                {
                    Id = ToLong(row["Id"]),
                    UserName = ToString(row["UserName"]),
                    Password = ToString(row["Password"]),
                    Name = ToString(row["Name"]),
                    LastName = ToString(row["LastName"]),
                    Email = ToString(row["Email"]),
                    RoleId = ToLong(row["RoleId"]),
                    Image = ToString(row["Image"]),
                    Creation = Convert.ToDateTime(row["Creation"]),
                    EndDate = !string.IsNullOrEmpty(ToString(row["EndDate"])) && !ToString(row["EndDate"]).Contains("1/1/1900") ? DateTime.Parse(ToString(row["EndDate"])) : (DateTime?)null,
                    Enabled = Convert.ToBoolean(ToString(row["Enabled"]))
                });
            }

            return entities;
        }

        public IEnumerable<Filter> GetUserFilter(long projectId, TransactionalContext transaction)
        {
            string query = string.Format("Select Distinct U.Id, U.UserName from Route R " +
                "Join [{0}] U on R.CensistId = U.Id " +
                "where R.ProjectId = {1}", TABLE_NAME, projectId);

            var result = ExecuteQuery(query);

            List<Filter> entities = new List<Filter>();

            foreach (DataRow row in result)
            {
                Filter itemRow = new Filter();

                itemRow.Key = ToLong(row["Id"]);
                itemRow.Value = ToString(row["UserName"]);

                entities.Add(itemRow);
            }

            return entities;
        }
    }
}
