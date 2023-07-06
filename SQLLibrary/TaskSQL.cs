using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using LogLibraryClassLibrary;
using static TaskClassLibrary.TaskOperators;
using TaskClassLibrary;

namespace SQL
{
    public class TaskSQL
    {
        public async Task<Tuple<bool, string, DataTable>> GetPendingTaskFromStoreAsync(int storeCode)
        {
            try
            {
                using (AutogestionTiendasEntities db = new AutogestionTiendasEntities())
                {
                    var query = await (from c in db.Task
                                       where c.task_audit_deleted == false &
                                       c.task_audit_deleted == false &
                                       c.task_status_id == (int)TaskOperators.EnumTaskStatusTask.PENDIENTE &
                                       c.Store.store_code == storeCode
                                       select new {
                                           ID = c.task_id,
                                           TIENDA = c.Store.store_code,
                                           ALIAS = c.StoreEmployee.stremp_alias,
                                           ESTATUS = c.StatusTask.status_task_description,
                                           DESCRIPCION = c.task_description,
                                           MENSAJE = c.task_moderator_message,
                                           FECHA = c.task_date
                                       }).ToListAsync(); 

                    Tuple<bool, string, DataTable> result = await System.Threading.Tasks.Task.Run(() => QueriesToDataTable.CreateDataTable(query));

                    if (result.Item1)
                    {
                        if (result.Item3.Rows.Count <= 0)
                        {
                            return new Tuple<bool, string, DataTable>(false, "Tareas vacias.", new DataTable());
                        }
                        else
                        {
                            return new Tuple<bool, string, DataTable>(true, "Operacion exitosa.", result.Item3);
                        }
                    }
                    else
                    {
                        return new Tuple<bool, string, DataTable>(false, result.Item2, new DataTable());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("Metodo: " + ex.TargetSite + ", Error: " + ex.Message.ToLower());
                return new Tuple<bool, string, DataTable>(false, "Error: " + ex.Message.ToLower(), new DataTable());
            }
        }

        public async Task<Tuple<bool, string, DataTable>> GetHistoricTaskAsync(int storeCodeInit, int storeCodeFinal, TaskOperators.EnumTaskStatusTask taskStatus, 
                                                                                TaskOperators.EnumTaskStatusLocal localStatus, DateTime dateEquals)
        {
            try
            {
                using (AutogestionTiendasEntities db = new AutogestionTiendasEntities())
                {
                    Tuple<bool, string, DataTable> result;

                    if (taskStatus == EnumTaskStatusTask.NONE && localStatus == EnumTaskStatusLocal.NONE)
                    {
                        var query = await (from c in db.Task
                                           where c.task_audit_deleted == false & 
                                           c.task_audit_deleted == false & 
                                           c.Store.store_code >= storeCodeInit
                                           & c.Store.store_code <= storeCodeFinal & 
                                           c.task_date.Day == dateEquals.Day & 
                                           c.task_date.Month == dateEquals.Month &
                                           c.task_date.Year == dateEquals.Year
                                           select new
                                           {
                                               ID = c.task_id,
                                               TIENDA = c.Store.store_code,
                                               ALIAS = c.StoreEmployee.stremp_alias,
                                               ESTATUS = c.StatusTask.status_task_description,
                                               DESCRIPCION = c.task_description,
                                               MENSAJE = c.task_moderator_message,
                                               ESTATUS_LOCAL = c.StatusLocal.status_local_description,
                                               MENSAJE_LOCAL = c.task_status_local_message,
                                               FECHA = c.task_date
                                           }).ToListAsync();

                        result = await System.Threading.Tasks.Task.Run(() => QueriesToDataTable.CreateDataTable(query));
                    }
                    else if(taskStatus == EnumTaskStatusTask.NONE && localStatus != EnumTaskStatusLocal.NONE)
                    {
                        var query = await (from c in db.Task
                                           where c.task_audit_deleted == false & 
                                           c.task_audit_deleted == false & 
                                           c.Store.store_code >= storeCodeInit & 
                                           c.Store.store_code <= storeCodeFinal & 
                                           c.task_status_local == (int)localStatus & 
                                           c.task_date.Day == dateEquals.Day & 
                                           c.task_date.Month == dateEquals.Month &
                                           c.task_date.Year == dateEquals.Year
                                           select new
                                           {
                                               ID = c.task_id,
                                               TIENDA = c.Store.store_code,
                                               ALIAS = c.StoreEmployee.stremp_alias,
                                               ESTATUS = c.StatusTask.status_task_description,
                                               DESCRIPCION = c.task_description,
                                               MENSAJE = c.task_moderator_message,
                                               ESTATUS_LOCAL = c.StatusLocal.status_local_description,
                                               MENSAJE_LOCAL = c.task_status_local_message,
                                               FECHA = c.task_date
                                           }).ToListAsync();

                        result = await System.Threading.Tasks.Task.Run(() => QueriesToDataTable.CreateDataTable(query));
                    }
                    else if (taskStatus != EnumTaskStatusTask.NONE && localStatus == EnumTaskStatusLocal.NONE)
                    {
                        var query = await (from c in db.Task
                                           where c.task_audit_deleted == false & 
                                           c.task_audit_deleted == false & 
                                           c.Store.store_code >= storeCodeInit & 
                                           c.Store.store_code <= storeCodeFinal & 
                                           c.task_status_id == (int)taskStatus & 
                                           c.task_date.Day == dateEquals.Day & 
                                           c.task_date.Month == dateEquals.Month &
                                           c.task_date.Year == dateEquals.Year
                                           select new
                                           {
                                               ID = c.task_id,
                                               TIENDA = c.Store.store_code,
                                               ALIAS = c.StoreEmployee.stremp_alias,
                                               ESTATUS = c.StatusTask.status_task_description,
                                               DESCRIPCION = c.task_description,
                                               MENSAJE = c.task_moderator_message,
                                               ESTATUS_LOCAL = c.StatusLocal.status_local_description,
                                               MENSAJE_LOCAL = c.task_status_local_message,
                                               FECHA = c.task_date
                                           }).ToListAsync();

                        result = await System.Threading.Tasks.Task.Run(() => QueriesToDataTable.CreateDataTable(query));
                    }
                    else
                    {
                        var query = await (from c in db.Task
                                           where c.task_audit_deleted == false & 
                                           c.task_audit_deleted == false & 
                                           c.Store.store_code >= storeCodeInit &
                                           c.Store.store_code <= storeCodeFinal & 
                                           c.task_status_id == (int)taskStatus & 
                                           c.task_status_local == (int)localStatus &
                                           c.task_date.Day == dateEquals.Day & 
                                           c.task_date.Month == dateEquals.Month &
                                           c.task_date.Year == dateEquals.Year
                                           select new
                                           {
                                               ID = c.task_id,
                                               TIENDA = c.Store.store_code,
                                               ALIAS = c.StoreEmployee.stremp_alias,
                                               ESTATUS = c.StatusTask.status_task_description,
                                               DESCRIPCION = c.task_description,
                                               MENSAJE = c.task_moderator_message,
                                               ESTATUS_LOCAL = c.StatusLocal.status_local_description,
                                               MENSAJE_LOCAL = c.task_status_local_message,
                                               FECHA = c.task_date
                                           }).ToListAsync();

                        result = await System.Threading.Tasks.Task.Run(() => QueriesToDataTable.CreateDataTable(query));
                    }
  
                    if (result.Item3.Rows.Count <= 0)
                    {
                        return new Tuple<bool, string, DataTable>(false, "No se encontaron Tareas.", new DataTable());
                    }
                    else
                    {
                        return new Tuple<bool, string, DataTable>(true, "Operacion exitosa.", result.Item3);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("Metodo: " + ex.TargetSite + ", Error: " + ex.Message.ToLower());
                return new Tuple<bool, string, DataTable>(false, "Error: " + ex.Message.ToLower(), new DataTable());
            }
        }

        public async Task<Tuple<bool, string, DataTable>> GetAllPendingTaskAsync()
        {
            try
            {
                using (AutogestionTiendasEntities db = new AutogestionTiendasEntities())
                {
                    var query = await (from c in db.Task
                                       where c.task_audit_deleted == false & 
                                       c.task_audit_deleted == false & 
                                       c.task_status_id == (int)TaskOperators.EnumTaskStatusTask.PENDIENTE
                                       select new
                                       {
                                           ID = c.task_id,
                                       }).ToListAsync();
                    Tuple<bool, string, DataTable> result = await System.Threading.Tasks.Task.Run(() => QueriesToDataTable.CreateDataTable(query));
                    if (result.Item1)
                    {
                        if (result.Item3.Rows.Count <= 0)
                        {
                            return new Tuple<bool, string, DataTable>(false, "Tareas vacias.", new DataTable());
                        }
                        else
                        {
                            return new Tuple<bool, string, DataTable>(true, "Operacion exitosa.", result.Item3);
                        }
                    }
                    else
                    {
                        return new Tuple<bool, string, DataTable>(false, result.Item2, new DataTable());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("Metodo: " + ex.TargetSite + ", Error: " + ex.Message.ToLower());
                return new Tuple<bool, string, DataTable>(false, "Error: " + ex.Message.ToLower(), new DataTable());
            }
        }

        public async Task<Tuple<bool, string>> SetTaskStatusAsync(int idTask, string messageModerator, int minutesAlive, EnumTaskStatusTask newStatus, int auditId)
        {
            try
            {
                using (AutogestionTiendasEntities db = new AutogestionTiendasEntities())
                {
                    var query = (from d in db.Task
                                     where d.task_id == idTask && 
                                     d.task_audit_deleted == false
                                     select d).FirstOrDefault();

                    query.task_status_id = (int)newStatus;
                    query.task_moderator_message = messageModerator;
                    query.task_audit_id = auditId;
                    query.task_audit_date = DateTime.Now;

                    if(newStatus == EnumTaskStatusTask.APROBADA)
                    {
                        query.task_token = Token.GetGeneratedNewToken(minutesAlive);
                    }
                    else
                    {
                        query.task_token = string.Empty;
                    }
                   
                    if (await db.SaveChangesAsync() > 0)
                    {
                        return new Tuple<bool, string>(true, "Operacion exitosa tarea cambiada al estatus: " + newStatus.ToString());
                    }
                    else
                    {
                        return new Tuple<bool, string>(false, "Operacion fallida al cambiar estatus de la tarea.");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("Metodo: " + ex.TargetSite + ", Error: " + ex.Message.ToLower());
                return new Tuple<bool, string>(false, "Error: " + ex.Message.ToLower());
            }
        }
    }
}
