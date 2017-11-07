using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebApplication1.Helpers;

namespace WebApplication1.Models
{
    public class ClientModel
    {
        public int ID { get; set; }
        public string Company { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string BillingInfo { get; set; }
        public string AdminEmail { get; set; }

        public bool Add()
        {
            string sql = @"INSERT INTO [dbo].[Clients]([Company],[Country],[Email],[Phone],[Address],[BillingInfo],[AdminEmail])
                           VALUES (@Company,@Country,@Email,@Phone,@Address,@BillingInfo,@AdminEmail)";

            var pl = new List<SqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@Company", this.Company));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Country", this.Country));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Email", this.Email));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Phone", this.Phone));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Address", this.Address));
            pl.Add(DatabaseHelper.CreateSqlParameter("@BillingInfo", this.BillingInfo));
            pl.Add(DatabaseHelper.CreateSqlParameter("@AdminEmail", this.AdminEmail));
            int r = DatabaseHelper.ExecuteNonQuery(sql, pl);

            if (r >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        } 
        public bool Update()
        {
            var pl = new List<SqlParameter>();
            pl.Add(new SqlParameter("ID", this.ID));
            int r = DatabaseHelper.ExecuteNonQuery("UPDATE dbo.Clients SET ? WHERE ID = @ID", pl);

            if (r >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Delete()
        {
            var pl = new List<SqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("ID", this.ID));
            int r = DatabaseHelper.ExecuteNonQuery("DELETE FROM dbo.Clients WHERE ID = @ID", pl);

            if (r >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<ClientModel> Get()
        {
            List<ClientModel> list = new List<ClientModel>();

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM dbo.Clients WITH (NOLOCK)");

            foreach (DataRow row in dt.Rows)
            {
                var client = new ClientModel();
                client.ID = Convert.ToInt32(row["Id"]);
                client.Company = Convert.ToString(row["Company"]);
                client.Country = Convert.ToString(row["Country"]);
                client.Email = Convert.ToString(row["Email"]);
                client.Phone = Convert.ToString(row["Phone"]);
                client.Address = Convert.ToString(row["Address"]);
                client.BillingInfo = Convert.ToString(row["BillingInfo"]);
                client.AdminEmail = Convert.ToString(row["AdminEmail"]);
                list.Add(client);
            }

            return list;
        }
    }
}