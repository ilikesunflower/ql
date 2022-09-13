using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using CMS_EF.DbContext;
using Microsoft.EntityFrameworkCore;

namespace CMS_Access.Extensions
{
    public class DatabaseUtils
    {
        public static IEnumerable DynamicListFromSql(ApplicationDbContext db, string sql,
            Dictionary<string, object> paramsList)
        {
            using var cmd = db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = sql;
            if (cmd.Connection != null && cmd.Connection.State != ConnectionState.Open)
            {
                cmd.Connection.Open();
            }

            foreach (KeyValuePair<string, object> p in paramsList)
            {
                DbParameter dbParameter = cmd.CreateParameter();
                dbParameter.ParameterName = p.Key;
                dbParameter.Value = p.Value;
                cmd.Parameters.Add(dbParameter);
            }

            using (var dataReader = cmd.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    var row = new ExpandoObject() as IDictionary<string, object>;
                    for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
                    {
                        row.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);
                    }

                    yield return row;
                }
            }
        }
    }
}