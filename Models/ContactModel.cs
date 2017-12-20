using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using WebApplication1.Helpers;

namespace WebApplication1.Models
{
    public class ContactModel
    {
        public int ID { get; set; }

        public int ClientID { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public string Phone { get; set; }

        public ContactModel() { }

        public ContactModel(DataRow row)
        {
            this.ID = Convert.ToInt32(row["Id"]);
            this.ClientID = Convert.ToInt32(row["ClientID"]);
            this.Email = Convert.ToString(row["Email"]);
            this.Phone = Convert.ToString(row["Phone"]);
            this.FirstName = Convert.ToString(row["FirstName"]);
            this.LastName = Convert.ToString(row["LastName"]);
        }

        public bool Add()
        {
            string sql = @"INSERT INTO [Clients]([Company],[Country],[Email],[Phone],[Address],[BillingInfo],[AdminEmail])
                           VALUES (@Company,@Country,@Email,@Phone,@Address,@BillingInfo,@AdminEmail)";

            int r = DatabaseHelper.ExecuteNonQuery(sql, GetParams());

            if (r >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<MySqlParameter> GetParams()
        {
            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", this.ID));
            pl.Add(DatabaseHelper.CreateSqlParameter("@ClientID", this.ClientID));
            pl.Add(DatabaseHelper.CreateSqlParameter("@FirstName", this.FirstName));
            pl.Add(DatabaseHelper.CreateSqlParameter("@LastName", this.LastName));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Email", this.Email));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Phone", this.Phone));

            return pl;
        }

        public bool Update()
        {
            string sql = @"UPDATE Clients SET   
                             [Company] = @Company
                            ,[Country] = @Country
                            ,[Email] = @Email
                            ,[Phone] = @Phone
                            ,[Address] = @Address
                            ,[BillingInfo] = @BillingInfo
                            ,[AdminEmail] = @AdminEmail
                            WHERE ID = @ID";

            int r = DatabaseHelper.ExecuteNonQuery(sql, GetParams());

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
            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("ID", this.ID));
            int r = DatabaseHelper.ExecuteNonQuery("DELETE FROM Clients WHERE ID = @ID", pl);

            if (r >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<ContactModel> Get()
        {
            List<ContactModel> list = new List<ContactModel>();

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Clients");

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ContactModel(row));
            }

            return list;
        }

        public static ContactModel GetByID(int id)
        {
            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", id));

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Clients WHERE ID = @ID", pl);

            var model = new ContactModel(dt.Rows[0]);

            return model;
        }
    }
}