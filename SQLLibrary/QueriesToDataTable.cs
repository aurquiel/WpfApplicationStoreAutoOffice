using LogLibraryClassLibrary;
using System;
using System.Collections.Generic;
using System.Data;

namespace SQL
{
    internal static class QueriesToDataTable
    {
        internal static Tuple<bool,string,DataTable> CreateDataTable<T>(IEnumerable<T> entities)
        {
            try
            {
                var dt = new DataTable();

                //creating columns
                foreach (var prop in typeof(T).GetProperties())
                {
                    dt.Columns.Add(prop.Name, prop.PropertyType);
                }

                //creating rows
                foreach (var entity in entities)
                {
                    var values = GetObjectValues(entity);
                    dt.Rows.Add(values);
                }

                return new Tuple<bool, string, DataTable>(true, "Operacion exitosa.", dt);
            }
            catch(Exception ex)
            {
                Logger.WriteToLog("Metodo: " + ex.TargetSite + ", Error: " + ex.Message.ToLower());
                return new Tuple<bool, string, DataTable>(false, "Error: " + ex.Message.ToLower(), new DataTable());
            }
        }

        internal static object[] GetObjectValues<T>(T entity)
        {
            var values = new List<object>();
            foreach (var prop in typeof(T).GetProperties())
            {
                values.Add(prop.GetValue(entity));
            }

            return values.ToArray();
        }
    }
}
