using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace WebApplication1.Helpers
{
    public class DatabaseHelper
    {
        private static string connectionString = "Server=tcp:casting305.database.windows.net,1433;Initial Catalog=Casting305;Persist Security Info=False;User ID=dbadmin;Password=Baddbpass1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static DataTable CallStoredProcedure(string procedureName, List<SqlParameter> paramList)
        {
            var table = new DataTable("Table");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string queryStatement = procedureName;

                using (var cmd = new SqlCommand(queryStatement, con))
                {
                    cmd.Parameters.AddRange(paramList.ToArray());
                    cmd.CommandType = CommandType.StoredProcedure;

                    var dap = new SqlDataAdapter(cmd);
                    con.Open();
                    dap.Fill(table);
                }
            }

            return table;
        }

        public static DataTable ExecuteQuery(string sql, List<SqlParameter> paramList = null)
        {
            var table = new DataTable("Table");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string queryStatement = sql;

                using (var cmd = new SqlCommand(queryStatement, con))
                {
                    if (paramList != null)
                    {
                        cmd.Parameters.AddRange(paramList.ToArray());
                    }

                    cmd.CommandType = CommandType.Text;

                    var dap = new SqlDataAdapter(cmd);
                    con.Open();
                    dap.Fill(table);
                }
            }

            return table;
        }

        public static int ExecuteNonQuery(string sql, List<SqlParameter> paramList = null)
        {
            int r = 0;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                using (var cmd = new SqlCommand(sql, con))
                {
                    if (paramList != null)
                    {
                        cmd.Parameters.AddRange(paramList.ToArray());
                    }

                    cmd.CommandType = CommandType.Text;

                    r = cmd.ExecuteNonQuery();
                }
            }

            return r;
        }

        public static SqlParameter CreateSqlParameter(string name, object value)
        {
            SqlParameter p = new SqlParameter();
            p.ParameterName = name;
            p.Value = value == null ? DBNull.Value : value;
            p.SqlDbType = SqlDbType.VarChar;


            return p;
        }
    }
}