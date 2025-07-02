using Marketaudit.DataAccess.Repositories;
using Marketaudit.Entities.Models.Response;
using Marketaudit.Service.Interfaces;
using MarketAudit.DataAccess.Interfaces;
using MarketAudit.DataAccess.Repositories;
using MarketAudit.Entities.Models;
using MarketAudit.Entities.Enum;
using MarketAudit.Entities.Models.Generic;
using MarketAudit.Entities.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using MarketAudit.Entities.Models.Request;
using System.Data;
using MarketAudit.Common.Log;
using System.Text.Json;
using MarketAudit.Service.Services;

namespace Marketaudit.Service.Services
{
    public class ProjectService : IProjectService
    {
        public IProjectRepository repository;
        public IRouteRepository routeRepository;
        public IPdvRepository pdvRepository;
        public IQuestionRepository questionRepository;
        public IResponseRepository responseRepository;
        public IProjectTypeRepository projectTypeRepository;
        public ICustomerRepository customerRepository;
        public IUserRepository userRepository;
        public IStateRepository stateRepository;
        public IQuestionTypeRepository questionTypeRepository;
        public IDataTypeRepository dataTypeRepository;
        public IPdvTypeRepository pdvTypeRepository;
        public IProjectQuestionRepository projectQuestionRepository;
        public IReportDetailRepository reportDetailRepository;
        public IProjectSizeRepository projectSizeRepository;

        public ProjectService()
        {
            repository = new ProjectRepository();
            routeRepository = new RouteRepository();
            pdvRepository = new PdvRepository();
            questionRepository = new QuestionRepository();
            responseRepository = new ResponseRepository();
            projectTypeRepository = new ProjectTypeRepository();
            customerRepository = new CustomerRepository();
            userRepository = new UserRepository();
            stateRepository = new StateRepository();
            questionTypeRepository = new QuestionTypeRepository();
            dataTypeRepository = new DataTypeRepository();
            pdvTypeRepository = new PdvTypeRepository();
            projectQuestionRepository = new ProjectQuestionRepository();
            reportDetailRepository = new ReportDetailRepository();
            projectSizeRepository = new ProjectSizeRepository();
        }

        public ResponseData Enable(long id)
        {
            var result = new ResponseData();
            var transaction = new TransactionalContext();

            try
            {
                Project project = repository.Get(id);

                if (project.StateId == (long)States.Create || project.StateId == (long)States.Desaporbado)
                {
                    if (routeRepository.GetRoute(id) == 0 || questionRepository.GetQuestions(id) == 0)
                    {
                        result.Message = string.Format("Para habilitar el proyecto {0} debe cargar PDV's y Preguntas", project.Name);
                    }
                    else
                    {
                        if (project.StateId == (long)States.Create)
                        {
                            reportDetailRepository.DeleteReport(id, transaction);
                        }

                        repository.Enable(id, (long)States.Aprobado, transaction);
                    }
                }
                else
                {
                    repository.Enable(id, (long)States.Desaporbado, transaction);
                }


                transaction.Commit();
                result.Status = string.IsNullOrEmpty(result.Message) ? "Ok" : "Validation";
                result.Message = string.IsNullOrEmpty(result.Message) ? "Se realizaron los cambios correctamente" : result.Message;
            }
            catch (Exception ex)
            {
                result.Status = "Error";
                result.Message = "Hubo un error al habilitar/deshabilitar registros";
                transaction.Rollback();
            }

            return result;

        }

        public ResponseData GetNewProject()
        {
            return new ResponseData()
            {
                Status = "Ok",
                Data = new Project
                {
                    StartDate = DateTime.Now.Date,
                    FinishDate = DateTime.Now.Date,
                    ProjectTypeList = projectTypeRepository.GetAll().Select(x => new KeyValueDto(x.Id, x.Descripcion)).ToList(),
                    CustomerList = customerRepository.GetCustomers().Select(x => new KeyValueDto(x.Id, x.Name)).ToList(),
                    ResponsableList = userRepository.GetResponsables().Select(x => new KeyValueDto(x.Id, x.Name)).ToList()
                }
            };
        }

        public ResponseData GetProject(long id)
        {
            var result = new ResponseData();
            var transaction = new TransactionalContext();

            try
            {
                Project project = repository.Get(id);
                project.ProjectTypeList = projectTypeRepository.GetAll().Select(x => new KeyValueDto(x.Id, x.Descripcion)).ToList();
                project.CustomerList = customerRepository.GetCustomers().Select(x => new KeyValueDto(x.Id, x.Name)).ToList();
                project.ResponsableList = userRepository.GetResponsables().Select(x => new KeyValueDto(x.Id, x.Name)).ToList();

                transaction.Commit();
                result.Data = project;
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

        public DataTableModel GetProjects(string states = null, string responsables = null)
        {
            return new DataTableModel
            {
                columns = new string[] { "ID", "NOMBRE", "TIPO DE PROYECTO", "RESPONSABLE", "CLIENTE", "CANTIDAD DE PDV'S", "CANTIDAD DE CENSISTAS", "CANTIDAD DE PREGUNTAS", "ESTADO" },
                data = repository.GetProjectsByGrid(states, responsables).Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.ProjectType,
                    x.Responsable,
                    x.Customer,
                    CantidadPdv = x.CantidadPdv.ToString(),
                    CantidadCensistas = x.CantidadCensistas.ToString(),
                    CantidadPreguntas = x.CantidadPreguntas.ToString(),
                    x.State
                }).ToArray()
            };
        }

        public DataTableModel GetProjectsDataTable(string states = null, string responsables = null)
        {
            var entity = new DataTableModel();

            var rows = repository.GetProjectsByGrid(states, responsables);
            entity.columns = new string[] { "ID", "NOMBRE", "TIPO DE PROYECTO", "RESPONSABLE", "CLIENTE", "CANTIDAD DE PDV'S", "CANTIDAD DE CENSISTAS", "CANTIDAD DE PREGUNTAS", "ESTADO" };

            DataTable dt = new DataTable();

            foreach (var item in entity.columns)
            {
                dt.Columns.Add(item);
            }

            var c = 0;
            entity.data = new object[rows.ToList().Count];

            foreach (var item in rows)
            {
                var row = dt.NewRow();
                row[0] = item.Id;
                row[1] = item.Name;
                row[2] = item.ProjectType;
                row[3] = item.Responsable;
                row[4] = item.Customer;
                row[5] = item.CantidadPdv;
                row[6] = item.CantidadCensistas;
                row[7] = item.CantidadPreguntas;
                row[8] = item.State;

                entity.data[c] = row.ItemArray;
                c++;
            }

            return entity;
        }

        public ResponseData GetProjectReports()
        {
            return new ResponseData
            {
                Status = "OK",
                Data = repository.GetProjectsReport().Select(x => new ProjectReport(x.Id, x.Name)).ToList()
            };
        }

        public DataTableModel GetQuestionByProjectId(long id)
        {
            return new DataTableModel
            {
                columns = new string[] { "ORDEN", "PREGUNTA", "DESCRIPCION", "TIPO DE DATO", "TIPO DE PREGUNTA", "REQUERIDO" },
                data = questionRepository.GetQuestionByProjectId(id).Select(x => new
                {
                    x.Order,
                    x.Question,
                    x.Description,
                    x.DataType,
                    x.QuestionType,
                    Required = x.Required ? "SI" : "NO"
                }).ToArray()
            };
        }

        public List<ProjectQuestionTemplateModel> GetQuestionTemplateByProjectId(long projectId)
        {
            return questionRepository.GetQuestionTemplateByProjectId(projectId).ToList();
        }

        public List<ProjectPdvTemplateModel> GetPdvTemplateByProjectId(long projectId)
        {
            return pdvRepository.GetPdvTemplateByProjectId(projectId).ToList();
        }

        public DataTableModel GetPdvByProjectId(long id)
        {
            return new DataTableModel
            {
                columns = new string[] { "NUMERO", "NOMBRE", "DESCRIPCION", "CUIT", "DIRECCION", "TIPO", "NOTAS", "RUTA", "CENSISTA" },
                data = pdvRepository.GetPdvByProyectId(id).Select(x => new
                {
                    x.Number,
                    x.Name,
                    x.Description,
                    x.Cuit,
                    x.Address,
                    x.Type,
                    x.Notes,
                    x.RouteName,
                    x.Censist
                }).ToArray()
            };
        }

        public IEnumerable<Projects> GetProjectsByUserId(long userId)
        {
            var projects = repository.GetProjectsByUserId(userId);
            var routes = routeRepository.GetRouteByUserId(userId);
            var pdvs = pdvRepository.GetPdvByUserId(userId);
            var questions = questionRepository.GetQuestionByUserId(userId);
            var responses = responseRepository.GetResponsesByUserId(userId);

            List<Projects> result = projects.Select(x => new Projects
            {
                Id = x.Id,
                Name = x.Name,
                Client = x.Client,
                ProjectType = x.ProjectType,
                ProjectStatus = x.ProjectStatus,
                TotalPdv = pdvRepository.GetCountPdvByProjectIdAndUserId(x.Id, userId),
                Routes = routes.Where(q => q.projectId == x.Id).Select(r => new Routes
                {
                    Id = r.Id,
                    Name = r.RouteName,
                    Description = r.RouteDescription,
                    Map = r.Map,
                    Pdvs = pdvs.Where(q => q.RouteId == r.Id).Select(p => new Pdv
                    {
                        Id = p.Id,
                        Number = p.PdvNumber,
                        Name = p.PdvName,
                        Description = p.PdvDescription,
                        Address = p.Address,
                        Completed = p.Completed
                    }).ToList()
                }).ToList(),

                Questions = questions.Where(q => q.ProjectId == x.Id).Select(q => new Questions
                {
                    Id = q.Id,
                    Question = q.Question,
                    Description = q.Description,
                    DataType = q.DataType,
                    QuestionType = q.QuestionType,
                    Required = q.Required,
                    QuestionImg = q.QuestionImg,
                    Order = q.Order,
                    Responses = responses.Where(t => t.QuestionId == q.QuestionId).Select(r => new Responses
                    {
                        Id = r.Id,
                        QuestionId = r.QuestionId,
                        Response = r.Response,
                        ValueLogic = r.ValueLogic,
                        IconResponse = r.IconResponse
                    }).ToList()
                }).ToList()

            }).ToList();

            projects = null;
            routes = null;
            pdvs = null;
            questions = null;
            responses = null;

            return result;
        }

        public ResponseData GetStates()
        {
            return new ResponseData()
            {
                Status = "Ok",
                Data = stateRepository.GetAll()
            };
        }

        public ResponseData Save(Project model)
        {
            var result = new ResponseData();
            var transaction = new TransactionalContext();
            var logger = new LoggerManager();
            logger.LogInfo(string.Format("SaveProjects - Inicio"));
            try
            {
                Project project;

                if (model.ProjectTypeId < 1)
                    return new ResponseData { Status = "Error", Message = "Seleccione tipo de proyecto" };

                if (model.CustomerId < 1)
                    return new ResponseData { Status = "Error", Message = "Seleccione cliente" };

                if (model.ResponsableId < 1)
                    return new ResponseData { Status = "Error", Message = "Seleccione responsable" };

                if (model.StartDate > model.FinishDate)
                    return new ResponseData { Status = "Error", Message = "La fecha de inicio debe ser menor que la fecha de fin" };

                if (model.Id == 0)
                {
                    project = new Project
                    {
                        Name = model.Name,
                        Description = model.Description,
                        ProjectTypeId = model.ProjectTypeId,
                        ResponsableId = model.ResponsableId,
                        CustomerId = model.CustomerId,
                        Creation = DateTime.Now,
                        StartDate = model.StartDate,
                        FinishDate = model.FinishDate,
                        Sas = false,
                        StateId = (long)States.Create
                };

                    repository.Insert(project, transaction);
                }
                else
                {
                    project = repository.Get(model.Id);
                    project.Name = model.Name;
                    project.Description = model.Description;
                    project.ProjectTypeId = model.ProjectTypeId;
                    project.ResponsableId = model.ResponsableId;
                    project.CustomerId = model.CustomerId;
                    project.StartDate = model.StartDate;
                    project.FinishDate = model.FinishDate;
                    project.Sas = model.Sas;
                    repository.Update(project, transaction);
                }

                transaction.Commit();
                result.Status = "Ok";
                result.Message = model.Id == 0 ? string.Format("Se creó el proyecto llamado {0}", model.Name) : "Se realizaron los cambios correctamente";
                logger.LogInfo(string.Format(result.Message));
                logger.LogInfo(string.Format("SaveProject {0} - Fin", model.Name));
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error al guardar el proyecto {0} - {1}", model.Name, ex.Message));
                result.Status = "Error";
                result.Message = "Hubo un error al guardar registros";
                transaction.Rollback();
            }

            return result;
        }

        public ResponseData SaveQuestions(long projectId, List<ImportQuestionModel> questions)
        {
            var result = new ResponseData();
            var transaction = new TransactionalContext();
            var logger = new LoggerManager();
            string jsonString = string.Empty;
            string Section = string.Empty;
            logger.LogInfo(string.Format("SaveQuestions - Inicio"));
            try
            {
                reportDetailRepository.DeleteReport(projectId, transaction);
                questionRepository.DeleteQuestionProject(projectId, transaction);
                List<Question> questionList = new List<Question>();

                foreach (var item in questions)
                {
                    jsonString = JsonSerializer.Serialize(item);
                    Section = "Pregunta";
                    Question question = new Question();
                    long questionId = 0;
                    question.QuestionName = item.Pregunta;
                    question.Description = item.Descripcion;
                    question.Required = item.Requerida;
                    question.QuestionTypeId = questionTypeRepository.Get(item.TipoPregunta, transaction).Id;
                    question.DataTypeId = dataTypeRepository.Get(item.TipoDato, transaction).Id;
                    question.Image = item.Miniatura;

                    questionId = questionRepository.Insert(question, transaction);

                    if (!string.IsNullOrEmpty(item.Respuestas))
                    {
                        Section = "Respuestas";
                        var responsesList = item.Respuestas.Split(';');

                        foreach (var resp in responsesList)
                        {
                            ResponseQuestion response = new ResponseQuestion();

                            long responseId = 0;
                            if (!responseRepository.Exist(resp.Trim(), transaction))
                            {

                                response.Response = resp.Trim();
                                responseId = responseRepository.Insert(response, transaction);
                            }
                            else
                            {
                                responseId = responseRepository.Get(resp.Trim(), transaction).Id;
                            }

                            QuestionResponse questionResponse = new QuestionResponse();
                            questionResponse.QuestionId = questionId;
                            questionResponse.ResponseId = responseId;

                            questionRepository.InsertQuestionResponse(questionResponse, transaction);
                        }
                    }

                    if (!string.IsNullOrEmpty(item.Disparadora) && !string.IsNullOrEmpty(item.Disparadora.Trim()))
                    {
                        Section = "Disparadora";
                        foreach (string respuesta in item.Disparadora.Trim().Split('|'))
                        {
                            var part = respuesta.Split(';');
                            string disparadora = part[0].Trim();
                            int saltar = part[1] != string.Empty ? Convert.ToInt32(part[1]) : 0;
                            ResponseQuestion response = responseRepository.Get(disparadora, transaction);
                            if (response != null)
                            {
                                QuestionLogic logic = new QuestionLogic();
                                logic.QuestionId = questionId;
                                logic.ResponseId = response.Id;
                                logic.Value = saltar;
                                questionRepository.InsertQuestionLogic(logic, transaction);

                            }
                        }
                    }

                    questionList.Add(question);

                    ProjectQuestion projectQuestion = new ProjectQuestion();
                    projectQuestion.ProjectId = projectId;
                    projectQuestion.QuestionId = questionId;
                    projectQuestion.Order = item.Orden;
                    Section = "Pregunta-Proyecto";
                    repository.InsertProjectQuestion(projectQuestion, transaction);
                }

                transaction.Commit();
                //repository.DropReportProjectTable(projectId);
                //repository.CreateReportProjectTable(projectId, questions);
                result.Status = "Ok";
                result.Message = "Se realizaron los cambios correctamente";
                logger.LogInfo(string.Format(result.Message));
                logger.LogInfo(string.Format("SaveQuestions - Fin"));
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error al guardar las preguntas - {0} - {1} - {2}", Section, jsonString, ex.Message));
                result.Status = "Error";
                result.Message = string.Format("Error al guardar las preguntas - {0} - {1} ", Section, ex.Message);
                transaction.Rollback();
            }

            return result;
        }

        public ResponseData SavePdvs(long projectId, List<ImportPdvModel> pdvs)
        {
            var result = new ResponseData();
            var transaction = new TransactionalContext();
            var logger = new LoggerManager();
            logger.LogInfo(string.Format("SavePdvs - Inicio"));
            string jsonString = string.Empty;
            string Section = string.Empty;
            try
            {
                List<Route> routeList = new List<Route>();
                foreach (var item in pdvs)
                {
                    jsonString = JsonSerializer.Serialize(item);
                    Route route = null;
                    long routeId = 0;
                    string rutaName = string.Format("Ruta {0}", item.Route);
                    User censist = userRepository.Get(item.Censist, transaction);
                    Section = "Censista";
                    if(censist == null)
                        throw new Exception("No existe el censista - "+ item.Censist);

                    Section = "Ruta";
                    if (!routeRepository.Exist(projectId, censist.Id, rutaName, transaction)
                        || !routeList.Any(x => x.ProjectId == projectId && x.CensistId == censist.Id && x.Name == rutaName))
                    {

                        route = new Route(item.Route, projectId, censist.Id, ValidateLanguage(item.Language));
                        routeId = routeRepository.Insert(route, transaction);
                    }
                    else
                    {
                        route = routeRepository.Get(projectId, censist.Id, rutaName, transaction);
                        routeId = route.Id;
                    }

                    if (!string.IsNullOrEmpty(item.Name))
                    {
                        Section = "Pdv";
                        PdvEntity pdv = new PdvEntity
                        {
                            Name = item.Name,
                            Description = string.Format("Descripcion punto de venta {0}", item.Name),
                            Number = item.Number,
                            Notes = item.Notes,
                            Cuit = item.Cuit,
                            Address = item.Address,
                            PdvTypeId = GetPdvTypeId(item.PdvType, transaction)
                        };

                        long pdvId = pdvRepository.Insert(pdv, transaction);

                        RoutePdv routePdv = new RoutePdv();
                        routePdv.RouteId = routeId;
                        routePdv.PdvId = pdvId;

                        Section = "Ruta-Pdv";
                        routeRepository.InsertRoutePdv(routePdv, transaction);
                    }

                    routeList.Add(route);
                }

                repository.FixDuplicateRoutes(projectId, transaction);

                transaction.Commit();
                result.Status = "Ok";
                result.Message = "Se realizaron los cambios correctamente";
                logger.LogInfo(string.Format(result.Message));
                logger.LogInfo(string.Format("SavePdvs - Fin"));
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error al guardar los pdvs - {0} - {1} - {2}", Section, jsonString, ex.Message));
                result.Status = "Error";
                result.Message = string.Format("Error al guardar los pdvs - {0} - {1} ", Section, ex.Message);
                transaction.Rollback();
            }

            return result;

        }

        public DataTableModel GetReportByProjectId(long projectId)
        {
            var transaction = new TransactionalContext();
            DataTable dt = new DataTable();
            var projectQuestion = projectQuestionRepository.GetColumnsReport(projectId,transaction);
            var reportDetail = reportDetailRepository.GetReportDetailsByProjectId(projectId);

            var cols = new string[projectQuestion.Rows.Count + 6];
            int c = 6;
            int d = 1;
            bool isFirst = true;
            var result = new DataTableModel();

            cols[0] = "Usuario";
            dt.Columns.Add(cols[0], Type.GetType("System.String"));

            cols[1] = "Codigo PDV";
            dt.Columns.Add(cols[1], Type.GetType("System.String"));

            cols[2] = "PDV";
            dt.Columns.Add(cols[2], Type.GetType("System.String"));

            cols[3] = "Ruta";
            dt.Columns.Add(cols[3], Type.GetType("System.String"));

            cols[4] = "Fecha";
            dt.Columns.Add(cols[4], Type.GetType("System.String"));

            cols[5] = "Hora";
            dt.Columns.Add(cols[5], Type.GetType("System.String"));


            foreach (DataRow item in projectQuestion.Rows)
            {
                if (cols.Any(q => q != null && q.ToUpper() == item["Question"].ToString().ToUpper()))
                {
                    cols[c] = item["Question"].ToString() + "_" + d;
                    d++;
                }
                else
                {
                    cols[c] = item["Question"].ToString();
                }

                dt.Columns.Add(cols[c], Type.GetType("System.String"));
                c++;
            }

            result.columns = cols;

            var reportIds = reportDetail.Select(q => q.ReportMasterId).Distinct();
            result.data = new object[reportIds.Count()];
            d = 0;

            foreach (var id in reportIds)
            {
                c = 6;

                var row = dt.NewRow();
                isFirst = true;
                //var orderList = new List<int>();

                foreach (var item in reportDetail.Where(q => q.ReportMasterId == id).OrderBy(q => q.Order))
                {
                        if (isFirst)
                        {
                            row[0] = item.UserName;
                            row[1] = item.PdvNumber;
                            row[2] = item.PdvName;
                            row[3] = item.Route;
                            row[4] = item.Creation.ToShortDateString();
                            row[5] = item.Creation.ToShortTimeString();
                            isFirst = false;
                        }

                        row[c] = !string.IsNullOrEmpty(item.Value) ? item.Value : "-";

                        c++;
                }


                result.data[d] = row.ItemArray;
                d++;
            }
            transaction.Commit();
            return result;


        }

        public void ProcessReportTable(long projectId)
        {
            var reportDetail = reportDetailRepository.GetReportDetailsByProjectId(projectId);
            bool isFirst = true;

            var reportIds = reportDetail.Select(q => q.ReportMasterId).Distinct();

            foreach (var id in reportIds)
            {
                isFirst = true;

                foreach (var item in reportDetail.Where(q => q.ReportMasterId == id).OrderBy(q => q.Order))
                {
                    if (isFirst && !repository.ExistsReportProjectTable(projectId, item.UserName, item.PdvNumber, item.PdvName, item.Route))
                    {
                        isFirst = false;
                        repository.InsertReport(projectId, item.UserName, item.PdvNumber, item.PdvName, item.Route, item.Creation.ToShortDateString(), item.Creation.ToShortTimeString());
                    }

                    repository.UpdateReport(projectId, item.UserName, item.PdvNumber, item.PdvName, item.Route, item.Value, item.Order.ToString());
                }
            }
        }

        public List<PhotosReport> GetPhotoByProjectId(long projectId, string users, string pdvs, string routes, string questions)
        {
            var transaction = new TransactionalContext();
            var result = reportDetailRepository.GetReportPhotosByProject(projectId, users, pdvs, routes, questions, transaction);
            transaction.Commit();
            return result;
        }

        public FilterPhotos GetFilterPhotoByProjectId(long projectId, string userId)
        {
            var transaction = new TransactionalContext();
            var result = new FilterPhotos();

            result.Users = userRepository.GetUserFilter(projectId, transaction).OrderBy(q => q.Value).ToList();
            result.Pdvs = pdvRepository.GetPdvFilter(projectId, userId, transaction).OrderBy(q => q.Value).ToList();
            result.Routes = routeRepository.GetRouteFilter(projectId, transaction).OrderBy(q => q.Value).ToList();
            result.Quesions = questionRepository.GetQuestionFilter(projectId, transaction).OrderBy(q => q.Value).ToList();
            transaction.Commit();
            return result;
        }

        public ResponseData DeleteProject(long[] ids)
        {
            var result = new ResponseData();
            var transaction = new TransactionalContext();

            try
            {
                foreach (long id in ids)
                {
                    repository.Delete(id, transaction);
                }

                transaction.Commit();
                result.Status = "Ok";
                result.Message = "Se eliminó correctamente";
            }
            catch (Exception ex)
            {
                result.Status = "Error";
                result.Message = "Hubo un error al eliminar proyectos";
                transaction.Rollback();
            }

            return result;
        }

        public void CreateTableReportProcess()
        {
            var projects = repository.GetProjectsByGrid("1,2", null);
            foreach(var item in projects)
            {
                var questions = questionRepository.GetQuestionByProjectId(item.Id).ToList();
                repository.DropReportProjectTable(item.Id);
                repository.CreateReportProjectTable(item.Id, questions);
            }
        }

        public void CreateTableReportProcessByProjectId(long projectId)
        {            
            var questions = questionRepository.GetQuestionByProjectId(projectId).ToList();
            repository.DropReportProjectTable(projectId);
            repository.CreateReportProjectTable(projectId, questions);
        }

        public DataTableModel GetReport(long projectId)
        {
            DataTable dt = new DataTable();
            var projectQuestion = projectQuestionRepository.GetColumnsReport(projectId,null);
            var reportDetail = repository.GetReport(projectId);

            var cols = new string[projectQuestion.Rows.Count + 6];
            int c = 6;
            int d = 1;
            var result = new DataTableModel();

            cols[0] = "Usuario";
            dt.Columns.Add(cols[0], Type.GetType("System.String"));

            cols[1] = "Codigo PDV";
            dt.Columns.Add(cols[1], Type.GetType("System.String"));

            cols[2] = "PDV";
            dt.Columns.Add(cols[2], Type.GetType("System.String"));

            cols[3] = "Ruta";
            dt.Columns.Add(cols[3], Type.GetType("System.String"));

            cols[4] = "Fecha";
            dt.Columns.Add(cols[4], Type.GetType("System.String"));

            cols[5] = "Hora";
            dt.Columns.Add(cols[5], Type.GetType("System.String"));


            foreach (DataRow item in projectQuestion.Rows)
            {
                if (cols.Any(q => q != null && q.ToUpper() == item["Question"].ToString().ToUpper()))
                {
                    cols[c] = item["Question"].ToString() + "_" + d;
                    d++;
                }
                else
                {
                    cols[c] = item["Question"].ToString();
                }

                dt.Columns.Add(cols[c], Type.GetType("System.String"));
                c++;
            }

            result.columns = cols;

            result.data = new object[reportDetail.Count()];
            d = 0;

            foreach (var item in reportDetail)
            {
                c = 0;
                var row = dt.NewRow();
                foreach(var itemCol in cols)
                {
                    row[c] = ((IDictionary<string, object>)item).Values.ToArray()[c] != null ? ((IDictionary<string, object>)item).Values.ToArray()[c] : "-";
                    c++;
                }

                result.data[d] = row.ItemArray;
                d++;
            }

            return result;
        }

        public bool ExistsTableReport(long projectId)
        {
            return repository.ExistsTableReport(projectId);
        }

        public void DeleteDuplicatePdvs(long projectId)
        {
            var transaction = new TransactionalContext();
            try
            {
                repository.DeleteDuplicatePdvs(projectId, transaction);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }

        public string ValidateLanguage(string language)
        {
            // Idiomas válidos
            string[] validLanguages = { "en", "pt", "es" };

            // Si el idioma seleccionado no está en la lista, seleccionar Español por defecto
            if (string.IsNullOrWhiteSpace(language) || !validLanguages.Contains(language.Trim()))
            {
                return "es";
            }

            // Devolver el idioma seleccionado tal como está
            return language.Trim();

        }

        public void InsertSinglePdv(PdvModelRequest model)
        {
            var transaction = new TransactionalContext();
            User censist = userRepository.Get(model.Censista, transaction);
            Route route;
            long routeId;

            if (!routeRepository.Exist(model.ProjectId, censist.Id, model.Ruta, transaction))
            {

                route = new Route(model.Ruta, model.ProjectId, censist.Id, ValidateLanguage("es"));
                routeId = routeRepository.Insert(route, transaction);
            }
            else
            {
                route = routeRepository.Get(model.ProjectId, censist.Id, model.Ruta, transaction);
                routeId = route.Id;
            }

            if (!string.IsNullOrEmpty(model.Nombre))
            {
                PdvEntity entity = new PdvEntity
                {
                    Name = model.Nombre,
                    Description = string.Format("Descripcion punto de venta {0}", model.Nombre),
                    Number = model.Numero,
                    Cuit = model.Cuit,
                    Address = model.Direccion,
                    PdvTypeId = GetPdvTypeId(model.Tipo, transaction),
                    Visible = model.Visible                    
                };

                long pdvId = pdvRepository.InsertSingle(entity, transaction);

                RoutePdv routePdv = new RoutePdv();
                routePdv.RouteId = routeId;
                routePdv.PdvId = pdvId;

                routeRepository.InsertRoutePdv(routePdv, transaction);
            }

            transaction.Commit();
        }

        public void SaveSingleQuestion(QuestionModelRequest model)
        {
            var transaction = new TransactionalContext();

            reportDetailRepository.DeleteReport(model.ProjectId, transaction);
            List<Question> questionList = new List<Question>();

            Question question = new Question();
            long questionId = 0;
            question.QuestionName = model.Pregunta;
            question.Description = model.Descripcion;
            question.Required = model.Requerida == "Si";
            question.QuestionTypeId = questionTypeRepository.Get(model.TipoPregunta, transaction).Id;
            question.DataTypeId = dataTypeRepository.Get(model.TipoDato, transaction).Id;
            question.Image = model.Miniatura;

            questionId = questionRepository.Insert(question, transaction);

            if (!string.IsNullOrEmpty(model.Respuestas))
            {
                var responsesList = model.Respuestas.Split(';');

                foreach (var resp in responsesList)
                {
                    ResponseQuestion response = new ResponseQuestion();

                    long responseId = 0;
                    if (!responseRepository.Exist(resp.Trim(), transaction))
                    {

                        response.Response = resp.Trim();
                        responseId = responseRepository.Insert(response, transaction);
                    }
                    else
                    {
                        responseId = responseRepository.Get(resp.Trim(), transaction).Id;
                    }

                    QuestionResponse questionResponse = new QuestionResponse();
                    questionResponse.QuestionId = questionId;
                    questionResponse.ResponseId = responseId;

                    questionRepository.InsertQuestionResponse(questionResponse, transaction);
                }
            }

            if (!string.IsNullOrEmpty(model.Disparadora) && !string.IsNullOrEmpty(model.Disparadora.Trim()))
            {
                foreach (string respuesta in model.Disparadora.Trim().Split('|'))
                {
                    var part = respuesta.Split(';');
                    string disparadora = part[0].Trim();
                    int saltar = part[1] != string.Empty ? Convert.ToInt32(part[1]) : 0;
                    ResponseQuestion response = responseRepository.Get(disparadora, transaction);
                    if (response != null)
                    {
                        QuestionLogic logic = new QuestionLogic();
                        logic.QuestionId = questionId;
                        logic.ResponseId = response.Id;
                        logic.Value = saltar;
                        questionRepository.InsertQuestionLogic(logic, transaction);

                    }
                }
            }

            questionList.Add(question);

            ProjectQuestion projectQuestion = new ProjectQuestion();
            projectQuestion.ProjectId = model.ProjectId;
            projectQuestion.QuestionId = questionId;
            projectQuestion.Order = model.Orden;
            repository.InsertProjectQuestion(projectQuestion, transaction);

            transaction.Commit();
        }

        private long GetPdvTypeId(string pdvType, TransactionalContext transaction)
        {
            var pdvTypeResult = pdvTypeRepository.Get(pdvType, transaction);

            if(pdvTypeResult != null)
            {
                return pdvTypeResult.Id;
            }
            else
            {
                throw new Exception(string.Format("El tipo de Pdv {0} no existe", pdvType));
            }
        }
    }
}
