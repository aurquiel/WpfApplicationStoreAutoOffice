using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using LogLibraryClassLibrary;
using TaskClassLibrary;

namespace SQL
{
    public class StoreSQL
    {
        public async Task<Tuple<bool, string, DataTable>> GetStoresAsync()
        {
            try
            {
                using (AutogestionTiendasEntities db = new AutogestionTiendasEntities())
                {
                    var query = await (from c in db.Store
                                       where c.store_audit_deleted == false
                                       select new {
                                           c.store_id,
                                           c.store_code,
                                           c.store_audit_id,
                                           c.store_audit_date,
                                           c.store_audit_deleted
                                       } ).OrderBy(s => s.store_code).ToListAsync();

                    Tuple<bool, string, DataTable> result = await System.Threading.Tasks.Task.Run(() => QueriesToDataTable.CreateDataTable(query));

                    if (result.Item1)
                    {
                        if (result.Item3.Rows.Count <= 0)
                        {
                            return new Tuple<bool, string, DataTable>(false, "Tiendas vacias.", new DataTable());
                        }
                        else
                        {
                            return new Tuple<bool, string, DataTable>(true, "Operacion exitosa.",result.Item3);
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

        public async Task<Tuple<bool, string, int>> GetNumberPendingTaskAsync(bool hasfilter, string initStore, string finalStore)
        {
            try
            {
                using (AutogestionTiendasEntities db = new AutogestionTiendasEntities())
                {
                    if(hasfilter)
                    {
                        int initStoreInt = Int32.Parse(initStore);
                        int finalStoreInt = Int32.Parse(finalStore);

                        var query = await (from c in db.Task
                                           where c.task_audit_deleted == false & 
                                           c.task_audit_deleted == false & 
                                           c.task_status_id == (int)TaskOperators.EnumTaskStatusTask.PENDIENTE & 
                                           c.Store.store_code >= initStoreInt & 
                                           c.Store.store_code <= finalStoreInt
                                           select new
                                       {
                                           c.task_status_id,
                                       }).CountAsync();

                        return new Tuple<bool, string, int>(true, "Numero de tareas obtenido correctamente.", query);
                    }
                    else
                    {
                        var query = await (from c in db.Task
                                           where c.task_audit_deleted == false & 
                                           c.task_audit_deleted == false & 
                                           c.task_status_id == (int)TaskOperators.EnumTaskStatusTask.PENDIENTE
                                       select new
                                       {
                                           c.task_status_id,
                                       }).CountAsync();

                        return new Tuple<bool, string, int>(true, "Numero de tareas obtenido correctamente.", query);
                    }                  
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("Metodo: " + ex.TargetSite + ", Error: " + ex.Message.ToLower());
                return new Tuple<bool, string, int>(false,"Error: " + ex.Message.ToLower(), 0);
            } 
        }

        public async Task<Tuple<bool, string>> EraseStoreAsync(int storeId, int auditId)
        {
            try
            {
                using (AutogestionTiendasEntities db = new AutogestionTiendasEntities())
                {

                    var query = await (from c in db.Store
                                       where c.store_id == storeId
                                       select c).FirstOrDefaultAsync();

                    query.store_audit_deleted = true;
                    query.store_audit_date = DateTime.Now;
                    query.store_audit_id = auditId;

                    if( await db.SaveChangesAsync() > 0)
                    {
                        return new Tuple<bool, string>(true, "Tienda eliminada exitosamente.");
                    }
                    else
                    {
                        return new Tuple<bool, string>(false, "Error al eliminar tienda.");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("Metodo: " + ex.TargetSite + ", Error: " + ex.Message.ToLower());
                return new Tuple<bool, string>(false, "Error: " + ex.Message.ToLower());
            }
        }

        public async Task<Tuple<bool, string>> InsertStoreAsync(int storeCode, int auditId)
        {
            try
            {
                using (AutogestionTiendasEntities db = new AutogestionTiendasEntities())
                {

                    if (await db.Store.Where(u => u.store_code == storeCode && u.store_audit_deleted == false).CountAsync() > 0)
                    {
                        return new Tuple<bool, string>(false, "Error: codigo de tienda " + storeCode + " ya en uso.");
                    }

                    var t = new Store //Make sure you have a table called store in DB
                    {
                        store_code = storeCode,
                        store_audit_id = auditId,
                        store_audit_date = DateTime.Now,
                        store_audit_deleted = false,
                    };

                    db.Store.Add(t);

                    if (await db.SaveChangesAsync() > 0)
                    {
                        return new Tuple<bool, string>(true, "Tienda agregada exitosamente.");
                    }
                    else
                    {
                        return new Tuple<bool, string>(false, "Error al agregar tienda.");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("Metodo: " + ex.TargetSite + ", Error: " + ex.Message.ToLower());
                return new Tuple<bool, string>(false, "Error: " + ex.Message.ToLower());
            }
        }

        public async Task<Tuple<bool, string, DataTable>> GetPendingStoresTaskAsync()
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
                                           c.Store.store_code,
                                       }).Distinct().OrderBy(s => s.store_code).ToListAsync();

                    Tuple<bool, string, DataTable> result = await System.Threading.Tasks.Task.Run(() => QueriesToDataTable.CreateDataTable(query));

                    if (result.Item1)
                    {
                        return new Tuple<bool, string, DataTable>(true, "Operacion exitosa tiendas con tareas pendientes obtenidas.", result.Item3);
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
    }
}
