using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using WebApplication1.Helpers;

namespace WebApplication1.Models
{
    public class ClientModel : BaseModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Company is required.")]
        public string Company { get; set; }

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

        public List<ContactModel> Contacts { get; set; }

        public static string RecordType { get { return "Client"; } }

        public ClientModel()
        {
            Contacts = new List<ContactModel>();
        }

        public ClientModel(DataRow row)
        {
            this.ID = Convert.ToInt32(row["Id"]);
            this.Company = Convert.ToString(row["Company"]);
            this.Email = Convert.ToString(row["Email"]);
            this.Phone = Convert.ToString(row["Phone"]);
            this.Address = Convert.ToString(row["Address"]);
            this.BillingInfo = Convert.ToString(row["BillingInfo"]);
            this.AdminEmail = Convert.ToString(row["AdminEmail"]);

            this.Country = new ListItemModel(Convert.ToString(row["Country"]));
        }

        public bool Add()
        {
            string sql = @"INSERT INTO Clients (Company,Country,Email,Phone,Address,BillingInfo,AdminEmail)
                           VALUES (@Company,@Country,@Email,@Phone,@Address,@BillingInfo,@AdminEmail); SELECT LAST_INSERT_ID()";

            this.ID = Convert.ToInt32(DatabaseHelper.ExecuteScalar(sql, GetParams()));

            if (this.ID > 0)
            {
                return SaveContacts();
            }
            else
            {
                return false;
            }
        } 

        public bool Update()
        {
            string sql = @"UPDATE Clients SET   
                             Company = @Company
                            ,Country = @Country
                            ,Email = @Email
                            ,Phone = @Phone
                            ,Address = @Address
                            ,BillingInfo = @BillingInfo
                            ,AdminEmail = @AdminEmail
                            WHERE ID = @ID";

            int r = DatabaseHelper.ExecuteNonQuery(sql, GetParams());

            if (r >= 1)
            {
                return SaveContacts();
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
                ContactModel.DeleteBySource(this.ID, RecordType);

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

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Clients");

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ClientModel(row));
            }

            return list;
        }

        public static ClientModel GetByID(int id)
        {
            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", id));

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Clients WHERE ID = @ID", pl);

            var model = new ClientModel(dt.Rows[0]);

            model.Contacts = ContactModel.GetBySource(id, RecordType);
            model.LoadLists();

            return model;
        }

        private List<MySqlParameter> GetParams()
        {
            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", this.ID));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Company", this.Company));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Country", this.Country.Value));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Email", this.Email));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Phone", this.Phone));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Address", this.Address));
            pl.Add(DatabaseHelper.CreateSqlParameter("@BillingInfo", this.BillingInfo));
            pl.Add(DatabaseHelper.CreateSqlParameter("@AdminEmail", this.AdminEmail));

            return pl;
        }

        public bool SaveContacts()
        {
            if (ContactModel.DeleteBySource(this.ID, RecordType))
            {
                if (Contacts == null)
                {
                    Contacts = new List<ContactModel>();
                }

                foreach (var contact in Contacts)
                {
                    contact.SourceID = this.ID;
                    contact.Type = RecordType;
                    if (!contact.Add())
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}