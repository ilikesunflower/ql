using System.Collections.Generic;
using System.Data;

namespace CMS_Lib.Extensions.DataTables
{
    public class DataSetLinqOperators
    {
        public static DataTable CopyToDataTable<T>(IEnumerable<T> source)
        {
            //you find the ObjectShredder implementation on the blog wich was linked.
            return new ObjectShredder<T>().Shred(source, null, null);
        }

        public static DataTable CopyToDataTable<T>(IEnumerable<T> source,
            DataTable table, LoadOption? options)
        {
            return new ObjectShredder<T>().Shred(source, table, options);
        }
    }
}