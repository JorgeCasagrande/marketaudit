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
using System.Security.Cryptography;
using System.Text;

namespace Marketaudit.Service.Services
{
    public class UserService : IUserService
    {
        public IUserRepository repository;
        public IRolRepository rolRepository;

        public UserService()
        {
            repository = new UserRepository();
            rolRepository = new RolRepository();
        }

        public ResponseData Enable(long[] ids)
        {
            var result = new ResponseData();
            var transaction = new TransactionalContext();

            try
            {
                foreach (long id in ids)
                {
                    User user = repository.Get(id);
                    user.Enabled = user.Enabled ? false : true;
                    user.EndDate = user.Enabled ? (DateTime?)null : DateTime.Now;
                    repository.Enable(user,transaction);
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

        public ResponseData Save(User model)
        {
            var result = new ResponseData();
            var transaction = new TransactionalContext();

            try
            {
                User user;

                if (model.Id == 0)
                {
                    user = new User();
                    user.UserName = model.UserName;
                    user.Password = !string.IsNullOrEmpty(model.Password) ? GenerateSHA256String(model.Password) : string.Empty;
                    user.Name = model.Name;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.RoleId = model.RoleId;
                    user.Enabled = true;
                    user.Image = model.Image;
                    user.Creation = DateTime.Now;
                    user.UserTest = model.UserTest;
                    repository.Insert(user, transaction);
                }
                else
                {
                    user = repository.Get(model.Id);
                    user.UserName = model.UserName;
                    if(!string.IsNullOrEmpty(model.Password))
                        user.Password = GenerateSHA256String(model.Password);
                    user.Name = model.Name;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.RoleId = model.RoleId;
                    user.Image = model.Image;
                    user.UserTest = model.UserTest;
                    repository.Update(user, transaction);
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

        public DataTableModel GetUsers(string roles = null, string states = null)
        {
            return new DataTableModel
            {
                columns = new string[] { "ID", "NOMBRE", "APELLIDO", "USUARIO", "ROL", "ESTADO", "FECHA ALTA", "FECHA BLOQUEO", "USUARIO DE PRUEBA" },
                data = repository.GetUsers(roles, states).Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.LastName,
                    x.UserName,
                    Rol = rolRepository.Get(x.RoleId).Descripcion,
                    Enable = x.Enabled ? "Habilitado" : "Deshabilitado",
                    Creation = x.Creation.Value.ToString("dd/MM/yyyy"),
                    EndDate = x.EndDate.HasValue ? x.EndDate.Value.ToString("dd/MM/yyyy") : string.Empty,
                    UserTest = x.UserTest ? "SI" : "NO"
                }).ToArray()
            };
        }

        public ResponseData GetResponsables()
        {
            return new ResponseData
            {
                Status = "OK",
                Data = repository.GetResponsables().Select(x => new KeyValueDto(x.Id, x.Name)).ToList()
            };
        }

        public ResponseData GetRoles()
        {
            return new ResponseData
            {
                Status = "OK",
                Data = rolRepository.GetRoles()
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

        public ResponseData GetNewUser()
        {
            return new ResponseData()
            {
                Status = "Ok",
                Data = new User { RoleList = rolRepository.GetRoles().Select(x => new KeyValueDto(x.Id, x.Descripcion)).ToList() }
            };
        }

        public ResponseData GetUser(long id)
        {
            var result = new ResponseData();
            var transaction = new TransactionalContext();

            try
            {
                User user = repository.Get(id);
                user.Password = string.Empty;
                user.RoleList = rolRepository.GetRoles().Select(x => new KeyValueDto(x.Id, x.Descripcion)).ToList();
                transaction.Commit();
                result.Data = user;
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
