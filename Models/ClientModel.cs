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
            pl.Add(new SqlParameter("@Company", this.Company));
            pl.Add(new SqlParameter("@Country", this.Country));
            pl.Add(new SqlParameter("@Email", this.Email));
            pl.Add(new SqlParameter("@Phone", this.Phone));
            pl.Add(new SqlParameter("@Address", this.Address));
            pl.Add(new SqlParameter("@BillingInfo", this.BillingInfo));
            pl.Add(new SqlParameter("@AdminEmail", this.AdminEmail));
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
            pl.Add(new SqlParameter("ID", this.ID));
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

                list.Add(client);
            }

            return list;
        }
    }
}