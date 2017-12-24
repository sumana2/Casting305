using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace WebApplication1.Helpers
{
    public class DatabaseHelper
    {
        private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString;

        public static DataTable ExecuteQuery(string sql, List<MySqlParameter> paramList = null)
        {
            var table = new DataTable("Table");

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    if (paramList != null)
                    {
                        cmd.Parameters.AddRange(paramList.ToArray());
                    }

                    cmd.CommandType = CommandType.Text;

                    var dap = new MySqlDataAdapter(cmd);
                    dap.Fill(table);
                }
            }

            return table;
        }

        public static int ExecuteNonQuery(string sql, List<MySqlParameter> paramList = null)
        {
            int r = 0;

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Prepare();
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

        public static object ExecuteScalar(string sql, List<MySqlParameter> paramList = null)
        {
            object obj;

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();

                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Prepare();
                    if (paramList != null)
                    {
                        cmd.Parameters.AddRange(paramList.ToArray());
                    }

                    cmd.CommandType = CommandType.Text;

                    obj = cmd.ExecuteScalar();
                }
            }

            return obj;
        }

        public static MySqlParameter CreateSqlParameter(string name, object value)
        {
            MySqlParameter p = new MySqlParameter();
            p.ParameterName = name;
            p.Value = value == null ? DBNull.Value : value;
            p.DbType = DbType.String;

            return p;
        }
    }
}