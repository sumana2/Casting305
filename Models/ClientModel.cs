using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using WebApplication1.Helpers;
using static WebApplication1.Models.ListItemModel;

namespace WebApplication1.Models
{
    public class ClientModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Company is required.")]
        public string Company { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public ListItemModel Country { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        [Display(Name = "Billing Info")]
        public string BillingInfo { get; set; }

        [Display(Name = "Admin Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string AdminEmail { get; set; }

        public bool Add()
        {
            string sql = @"INSERT INTO [dbo].[Clients]([Company],[Country],[Email],[Phone],[Address],[BillingInfo],[AdminEmail])
                           VALUES (@Company,@Country,@Email,@Phone,@Address,@BillingInfo,@AdminEmail)";

            var pl = new List<SqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@Company", this.Company));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Country", this.Country.Value));
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
            string sql = @"UPDATE dbo.Clients SET   
                             [Company] = @Company
                            ,[Country] = @Country
                            ,[Email] = @Email
                            ,[Phone] = @Phone
                            ,[Address] = @Address
                            ,[BillingInfo] = @BillingInfo
                            ,[AdminEmail] = @AdminEmail
                            WHERE ID = @ID";

            var pl = new List<SqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", this.ID));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Company", this.Company));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Country", this.Country.Value));
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
                client.Email = Convert.ToString(row["Email"]);
                client.Phone = Convert.ToString(row["Phone"]);
                client.Address = Convert.ToString(row["Address"]);
                client.BillingInfo = Convert.ToString(row["BillingInfo"]);
                client.AdminEmail = Convert.ToString(row["AdminEmail"]);

                client.Country = new ListItemModel(Convert.ToString(row["Country"]));

                list.Add(client);
            }

            return list;
        }

        public static ClientModel GetByID(int id)
        {
            ClientModel client = new ClientModel();

            var pl = new List<SqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", id));

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM dbo.Clients WITH (NOLOCK) WHERE ID = @ID", pl);

            foreach (DataRow row in dt.Rows)
            {
                client.ID = Convert.ToInt32(row["Id"]);
                client.Company = Convert.ToString(row["Company"]);
                client.Email = Convert.ToString(row["Email"]);
                client.Phone = Convert.ToString(row["Phone"]);
                client.Address = Convert.ToString(row["Address"]);
                client.BillingInfo = Convert.ToString(row["BillingInfo"]);
                client.AdminEmail = Convert.ToString(row["AdminEmail"]);

                client.Country = new ListItemModel(Lists.Country, Convert.ToString(row["Country"]));
            }

            return client;
        }

        public void LoadLists()
        {
            if (this.Country == null)
                this.Country = new ListItemModel(Lists.Country);

            this.Country.LoadList();
        }
    }
}