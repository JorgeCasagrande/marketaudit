using Marketaudit.DataAccess.Repositories;
using Marketaudit.Entities.Models.Response;
using Marketaudit.Service.Interfaces;
using MarketAudit.DataAccess.Interfaces;
using MarketAudit.DataAccess.Repositories;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Models.Generic;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marketaudit.Service.Services
{
    public class CustomerService : ICustomerService
    {
        public ICustomerRepository repository;
        
        public CustomerService()
        {
            repository = new CustomerRepository();
        }

        public ResponseData Enable(long[] ids)
        {
            var result = new ResponseData();
            var transaction = new TransactionalContext();

            try
            {
                foreach (long id in ids)
                {
                    Customer customer = repository.GetCustomer(id, transaction);
                    repository.Enable(id, customer.Enable ? 0 : 1, transaction);
                }

                transaction.Commit();
                result.Status = "Ok";
                result.Message = "Se realizaron los cambios correctamente";
            }
            catch (Exception ex)
            {
                result.Status = "Error";
                result.Message = "Hubo un error al habilitar/deshabilitar registros";
                transaction.Rollback();
            }

            return result;
        }

        public ResponseData Delete(long[] ids)
        {
            var result = new ResponseData();
            var transaction = new TransactionalContext();

            try
            {
                foreach (long id in ids)
                {
                    repository.Delete(id,transaction);
                }

                transaction.Commit();
                result.Status = "Ok";
                result.Message = "Se eliminó correctamente";
            }
            catch (Exception ex)
            {
                result.Status = "Error";
                result.Message = "Hubo un error al eliminar registros";
                transaction.Rollback();
            }

            return result;
        }

        public ResponseData Save(Customer model)
        {
            var result = new ResponseData();
            var transaction = new TransactionalContext();

            try
            {
                Customer customer;

                if (model.Id == 0)
                {
                    customer = new Customer();
                    customer.Name = model.Name;
                    customer.Description = model.Description;
                    customer.Image = model.Image;
                    customer.Enable = true;
                    repository.Insert(customer, transaction);
                }
                else
                {
                    customer = repository.GetCustomer(model.Id, transaction);
                    customer.Name = model.Name;
                    customer.Description = model.Description;
                    customer.Image = model.Image;
                    repository.Update(customer, transaction);
                }

                transaction.Commit();
                result.Status = "Ok";
                result.Message = "Se realizaron los cambios correctamente";
            }
            catch (Exception ex)
            {
                result.Status = "Error";
                result.Message = "Hubo un error al guardar registros";
                transaction.Rollback();
            }

            return result;
        }


        public DataTableModel GetCustomers(string states = null)
        {
            return new DataTableModel
            {
                columns = new string[] { "ID", "NOMBRE", "DESCRIPCION", "LOGO", "ESTADO" },
                data = repository.GetCustomers(states).Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Description,
                    x.Image,
                    Enable = x.Enable ? "Habilitado" : "Deshabilitado"
                }).ToArray()
            };
        }

        public ResponseData GetStates()
        {
            return new ResponseData
            {
                Status = "OK",
                Data = new List<KeyValuePair<int, string>>
                {
                    new KeyValuePair<int, string>(1, "Habilitado"),
                    new KeyValuePair<int, string>(0, "Deshabilitado")
                }
            };
        }

        public ResponseData GetNewCustomer()
        {
            return new ResponseData()
            {
                Status = "Ok",
                Data = new Customer { }
            };
        }

        public ResponseData GetCustomer(long id)
        {
            var result = new ResponseData();
            var transaction = new TransactionalContext();

            try
            {
                Customer customer = repository.GetCustomer(id, transaction);
                Customer updateCustomer = new Customer
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Description = customer.Description,
                    Enable = customer.Enable,
                    Image = customer.Image
                };
                
                transaction.Commit();
                result.Data = updateCustomer;
                result.Status = "Ok";
                result.Message = "Ok";
            }
            catch (Exception ex)
            {
                result.Status = "Error";
                result.Message = "Hubo un error al traer el registro";
                transaction.Rollback();
            }

            return result;

        }
    }
}
