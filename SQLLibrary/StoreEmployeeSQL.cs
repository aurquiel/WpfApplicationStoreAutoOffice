using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using LogLibraryClassLibrary;

namespace SQL
{
    public class StoreEmployeeSQL
    {
        public async Task<Tuple<bool, string, DataTable>> GetStoresEmployeeAsync()
        {
            try
            {
                using (AutogestionTiendasEntities db = new AutogestionTiendasEntities())
                {
                    var query = await (from c in db.StoreEmployee
                                       where c.stremp_audit_deleted == false
                                       select new {
                                           c.stremp_id,
                                           c.stremp_alias,
                                           c.Store.store_code,
                                           c.stremp_audit_id,
                                           c.stremp_audit_date,
                                           c.stremp_audit_deleted
                                       }).OrderBy(o => o.stremp_alias).ToListAsync();

                    Tuple<bool, string, DataTable> result = await System.Threading.Tasks.Task.Run(() => QueriesToDataTable.CreateDataTable(query));

                    if (result.Item1)
                    {
                        if (result.Item3.Rows.Count <= 0)
                        {
                            return new Tuple<bool, string, DataTable>(false, "Usuarios de Tiendas vacios.", new DataTable());
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

        public async Task<Tuple<bool, string>> StoreHasEmployeesAsync(int storeId, int storeCode)
        {
            try
            {
                using (AutogestionTiendasEntities db = new AutogestionTiendasEntities())
                {
                    var query = await (from c in db.StoreEmployee
                                       where c.Store.store_audit_deleted == false & 
                                       c.Store.store_id == storeId & 
                                       c.stremp_audit_deleted == false
                                       select new {
                                           c.stremp_alias
                                       }).ToListAsync();

                    Tuple<bool, string, DataTable> result = await System.Threading.Tasks.Task.Run(() => QueriesToDataTable.CreateDataTable(query));

                    if (result.Item3.Rows.Count > 0)
                    {
                        string users = string.Empty;
                        foreach(DataRow user in result.Item3.Rows)
                        {
                            users += user[0].ToString() + ",";
                        }
                        users = users.TrimEnd(',');

                        return new Tuple<bool, string>(true, "Error hay usuarios afiliados a la Tienda:" + storeCode + ". Usuarios: " + users + ".");
                    }
                    else
                    {
                        return new Tuple<bool, string>(false, "No hay usuarios afiliados a la tienda " + storeId +".");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("Metodo: " + ex.TargetSite + ", Error: " + ex.Message.ToLower());
                return new Tuple<bool, string>(true, "Error: " + ex.Message.ToLower());
            }
        }

        public async Task<Tuple<bool, string>> EditUserStoreAsync(int userStoreId, string userStoreAlias, int userStoreCodeId, int auditId)
        {
            try
            {
                using (AutogestionTiendasEntities db = new AutogestionTiendasEntities())
                {
                    var query = await (from c in db.StoreEmployee
                                       where c.stremp_id == userStoreId & 
                                       c.stremp_audit_deleted == false
                                       select c).FirstOrDefaultAsync();

                    query.stremp_alias = userStoreAlias;
                    query.stremp_store_id = userStoreCodeId;
                    query.stremp_audit_deleted = false;
                    query.stremp_audit_date = DateTime.Now;
                    query.stremp_audit_id = auditId;

                    if (await db.SaveChangesAsync() > 0)
                    {
                        return new Tuple<bool, string>(true, "Usuario de Tienda editado con exito.");
                    }
                    else
                    {
                        return new Tuple<bool, string>(false, "Error al editar usuario de tienda.");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("Metodo: " + ex.TargetSite + ", Error: " + ex.Message.ToLower());
                return new Tuple<bool, string>(false, "Error: " + ex.Message.ToLower());
            }
        }

        public async Task<Tuple<bool, string>> EraseUserStoreAsync(int userStoreId, int auditId)
        {
            try
            {
                using (AutogestionTiendasEntities db = new AutogestionTiendasEntities())
                {
                    var query = await (from c in db.StoreEmployee
                                       where c.stremp_id == userStoreId & 
                                       c.stremp_audit_deleted == false
                                       select c).FirstOrDefaultAsync();

                    query.stremp_audit_deleted = true;
                    query.stremp_audit_date = DateTime.Now;
                    query.stremp_audit_id = auditId;

                    if (await db.SaveChangesAsync() > 0)
                    {
                        return new Tuple<bool, string>(true, "Usuario de Tienda eliminado con exito.");
                    }
                    else
                    {
                        return new Tuple<bool, string>(false, "Error al eliminar usuario de tienda.");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("Metodo: " + ex.TargetSite + ", Error: " + ex.Message.ToLower());
                return new Tuple<bool, string>(false, "Error: " + ex.Message.ToLower());
            }
        }

        public async Task<Tuple<bool, string>> InsertEmployStoreAsync(string alias, int storeCodeId, int auditId)
        {
            try
            {
                using (AutogestionTiendasEntities db = new AutogestionTiendasEntities())
                {
                    if (await db.StoreEmployee.Where(u => u.stremp_alias == alias && u.Store.store_code == storeCodeId &&
                            u.stremp_audit_deleted == false).CountAsync() > 0)
                    {
                        return new Tuple<bool, string>(false, "Error: alias " + alias + " ya en uso, en la tienda: " + storeCodeId);
                    }

                    var t = new StoreEmployee
                    {
                        stremp_alias = alias,
                        stremp_store_id = storeCodeId,
                        stremp_audit_id = auditId,
                        stremp_audit_date = DateTime.Now,
                        stremp_audit_deleted = false,
                    };

                    db.StoreEmployee.Add(t);

                    if (await db.SaveChangesAsync() > 0)
                    {
                        return new Tuple<bool, string>(true, "Usuario de tienda agregada exitosamente.");
                    }
                    else
                    {
                        return new Tuple<bool, string>(false, "Error al agregar usuario de tienda.");
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
