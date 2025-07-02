using Marketaudit.Entities.Models.Response;
using MarketAudit.DataAccess.Interfaces;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Marketaudit.DataAccess.Repositories
{
    public class CustomerRepository : DataBaseRepository, ICustomerRepository
    {
        public CustomerRepository()
        {
            TABLE_NAME = "CUSTOMER";
        }

        public void Delete(long id, ITransactionalContext transaction)
        {
            string query = string.Format("Delete Customer WHERE Id = {0}", id);

            ExecuteQuery(query, transaction);
        }

        public void Enable(long id,int enable, ITransactionalContext transaction)
        {
            string query = string.Format("UPDATE Customer SET [Enable] = '{0}' WHERE Id = {1}", enable, id);

            ExecuteQuery(query, transaction);
        }

        public Customer GetCustomer(long id, ITransactionalContext transaction)
        {
            string whereClause = string.Format("Where c.Id ={0}", id);
            string query = string.Format("SELECT c.Id, c.Name, c.Description, c.Image, c.Enable FROM {0} c {1} ", TABLE_NAME, whereClause);

            var result = ExecuteQuery(query, transaction);

            var row = result[0];

            return new Customer
            {
                Id = ToLong(row["Id"]),
                Name = ToString(row["Name"]),
                Description = ToString(row["Description"]),
                Image = ToString(row["Image"]),
                Enable = Convert.ToBoolean(ToString(row["Enable"]))
            };
        }

        public IEnumerable<Customer> GetCustomers(string states)
        {
            string whereClause = string.Empty;
            if (!string.IsNullOrEmpty(states) && states.Length > 0)
            {
                whereClause = string.Format("Where c.Enable in ({0})", states); 
            }
            string query = string.Format("SELECT c.Id, c.Name, c.Description, c.Image, c.Enable FROM {0} c {1} Order by c.Name asc ", TABLE_NAME, whereClause);

            var result = ExecuteQuery(query);

            List<Customer> entities = new List<Customer>();

            foreach (DataRow row in result)
            {

                Customer itemRow = new Customer();
                itemRow.Id = ToLong(row["Id"]);
                itemRow.Name = ToString(row["Name"]);
                itemRow.Description = ToString(row["Description"]);
                itemRow.Image = ToString(row["Image"]) != null ? ToString(row["Image"]) : string.Empty;
                itemRow.Enable = Convert.ToBoolean(ToString(row["Enable"]));
                          
                entities.Add(itemRow);
            }

            return entities;
        }

        public IEnumerable<Customer> GetCustomers(bool enable = true)
        {
            string whereClause =  string.Format("Where c.Enable in ({0})", enable ? 1 : 0);
            
            string query = string.Format("SELECT c.Id, c.Name, c.Description, c.Image, c.Enable FROM {0} c {1} Order by c.Name asc ", TABLE_NAME, whereClause);

            var result = ExecuteQuery(query);

            List<Customer> entities = new List<Customer>();

            foreach (DataRow row in result)
            {

                Customer itemRow = new Customer();
                itemRow.Id = ToLong(row["Id"]);
                itemRow.Name = ToString(row["Name"]);
                itemRow.Description = ToString(row["Description"]);
                itemRow.Image = ToString(row["Image"]) != null ? ToString(row["Image"]) : string.Empty;
                itemRow.Enable = Convert.ToBoolean(ToString(row["Enable"]));

                entities.Add(itemRow);
            }

            return entities;
        }

        public void Update(Customer model, ITransactionalContext transaction)
        {
            string query = string.Format("UPDATE Customer SET [Name] = '{0}', [Description] = '{1}', [Image] = '{2}' WHERE Id = {3}", RemoveCharacterInvalid(model.Name), RemoveCharacterInvalid(model.Description), model.Image, model.Id);

            ExecuteQuery(query, transaction);
        }

        public void Insert(Customer model, ITransactionalContext transaction)
        {
            string query = string.Format("INSERT INTO Customer ([Name], [Description], [Image], [Enable]) VALUES ('{0}','{1}','{2}','{3}')", RemoveCharacterInvalid(model.Name), RemoveCharacterInvalid(model.Description), model.Image, model.Enable);

            ExecuteQuery(query, transaction);
        }
    }
}
