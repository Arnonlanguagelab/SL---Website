using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Web;
using Microsoft.Practices.EnterpriseLibrary.Data;



public static class DBCommon
{
    public static void DBLog(string _message, ExpLogType _type, string _source)
    {
        try
        {
            // create split message list with a maximum length according to the [dbo].[log].[log_message] length
            List<string> splitMessage = Common.SplitStringByLength(_message, Common.SQL_LOG_MESSAGE_MAX_LENGTH);

            foreach (string msg in splitMessage)
            {
                Database db = DatabaseFactory.CreateDatabase();
                DbCommand logCommand = db.GetStoredProcCommand("LogMessage");
                db.AddInParameter(logCommand, "message", DbType.String, msg);
                db.AddInParameter(logCommand, "type", DbType.Int16, _type);
                db.AddInParameter(logCommand, "source", DbType.String, _source);
                db.AddInParameter(logCommand, "log_time", DbType.DateTime, Common.GetIsraelTimeNow());

                db.ExecuteNonQuery(logCommand);
            }
        }
        catch { }
    }

    // Bulk inserts
    public static bool BulkInsert<T>(IList<T> _rows, string _tableName)
    {
        try
        {
            DataTable table = ToDataTable<T>(_rows);

            using (SqlConnection destinationConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["expdb"].ConnectionString))
            {
                destinationConnection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection))
                {
                    bulkCopy.DestinationTableName = _tableName;
                    bulkCopy.WriteToServer(table);
                }
                destinationConnection.Close();
            }
            return true;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            return false;
        }
    }
    public static DataTable ToDataTable<T>(this IList<T> data)
    {
        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
        DataTable table = new DataTable();
        for (int i = 0; i < props.Count; i++)
        {
            PropertyDescriptor prop = props[i];
            table.Columns.Add(prop.Name, prop.PropertyType);
        }
        object[] values = new object[props.Count];
        foreach (T item in data)
        {
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = props[i].GetValue(item);
            }
            table.Rows.Add(values);
        }
        return table;
    }
}