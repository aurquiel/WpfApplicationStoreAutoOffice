using LogLibraryClassLibrary;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

namespace SQL
{
    public class UserSQL
    {
        public async Task<Tuple<bool, string, DataTable>> GetUsersAsync()
        {
            try
            {
                using (AutogestionTiendasEntities db = new AutogestionTiendasEntities())
                {
                    var query = await (from c in db.User
                                       where c.user_audit_deleted == false
                                       select new {
                                           c.user_id,
                                           c.user_alias,
                                           c.user_audit_id,
                                           c.user_audit_date,
                                           c.user_audit_deleted
                                       }).OrderBy(o => o.user_alias).ToListAsync();

                    Tuple<bool, string, DataTable> result = await System.Threading.Tasks.Task.Run(() => QueriesToDataTable.CreateDataTable(query));

                    if (result.Item1)
                    {
                        if(result.Item3.Rows.Count <= 0)
                        {
                            return new Tuple<bool, string, DataTable>(false, "Usuarios vacios.", new DataTable());
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
            catch(Exception ex)
            {
                Logger.WriteToLog("Metodo: " + ex.TargetSite + ", Error: " + ex.Message.ToLower());
                return new Tuple<bool, string, DataTable>(false, "Error: " + ex.Message.ToLower(), new DataTable());
            }
        }

        public async Task<Tuple<bool, string>> EraseUserAsync(int userId, int auditId)
        {
            try
            {
                using (AutogestionTiendasEntities db = new AutogestionTiendasEntities())
                {

                    var query = await (from c in db.User
                                       where c.user_id == userId
                                       select c).FirstOrDefaultAsync();

                    query.user_audit_deleted = true;
                    query.user_audit_date= DateTime.Now;
                    query.user_audit_id = auditId;

                    if (await db.SaveChangesAsync() > 0)
                    {
                        return new Tuple<bool, string>(true, "Usuario eliminado exitosamente.");
                    }
                    else
                    {
                        return new Tuple<bool, string>(false, "Error al eliminar usuario.");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("Metodo: " + ex.TargetSite + ", Error: " + ex.Message.ToLower());
                return new Tuple<bool, string>(false, "Error: " + ex.Message.ToLower());
            }
        }

        public async Task<Tuple<bool, string>> EditUserAsync(int userId, string newAlias, int auditId)
        {
            try
            {
                using (AutogestionTiendasEntities db = new AutogestionTiendasEntities())
                {

                    var query = await (from c in db.User
                                       where c.user_id == userId
                                       select c).FirstOrDefaultAsync();

                    query.user_alias = newAlias;
                    query.user_audit_deleted = false;
                    query.user_audit_date = DateTime.Now;
                    query.user_audit_id = auditId;

                    if (await db.SaveChangesAsync() > 0)
                    {
                        return new Tuple<bool, string>(true, "Usuario editado exitosamente.");
                    }
                    else
                    {
                        return new Tuple<bool, string>(false, "Error al editar usuario.");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteToLog("Metodo: " + ex.TargetSite + ", Error: " + ex.Message.ToLower());
                return new Tuple<bool, string>(false, "Error: " + ex.Message.ToLower());
            }
        }

        public async Task<Tuple<bool, string>> InsertUserAsync(string alias, int auditId)
        {
            try
            {
                using (AutogestionTiendasEntities db = new AutogestionTiendasEntities())
                {

                    if(await db.User.Where(u => u.user_alias == alias && u.user_audit_deleted == false).CountAsync() > 0)
                    {
                        return new Tuple<bool, string>(false, "Error: alias "+ alias + " ya en uso.");
                    }

                    var t = new User 
                    {
                        user_alias = alias,
                        user_audit_id = auditId,
                        user_audit_date = DateTime.Now,
                        user_audit_deleted = false,
                    };

                    db.User.Add(t);

                    if (await db.SaveChangesAsync() > 0)
                    {
                        return new Tuple<bool, string>(true, "Usuario agregado exitosamente.");
                    }
                    else
                    {
                        return new Tuple<bool, string>(false, "Error al agregar usuario.");
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
