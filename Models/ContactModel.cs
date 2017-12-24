﻿using MySql.Data.MySqlClient;
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

        public int SourceID { get; set; }

        public string Type { get; set; }

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

        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }

        public string FullNameNoSpace { get { return string.Format("{0}{1}", FirstName, LastName); } }

        public ContactModel(DataRow row)
        {
            this.ID = Convert.ToInt32(row["Id"]);
            this.SourceID = Convert.ToInt32(row["SourceID"]);
            this.Type = Convert.ToString(row["Type"]);
            this.Email = Convert.ToString(row["Email"]);
            this.Phone = Convert.ToString(row["Phone"]);
            this.FirstName = Convert.ToString(row["FirstName"]);
            this.LastName = Convert.ToString(row["LastName"]);
        }

        public bool Add()
        {
            string sql = @"INSERT INTO Contacts(SourceID,Type,Email,Phone,FirstName,LastName)
                           VALUES (@SourceID,@Type,@Email,@Phone,@FirstName,@LastName)";

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
            pl.Add(DatabaseHelper.CreateSqlParameter("@SourceID", this.SourceID));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Type", this.Type));
            pl.Add(DatabaseHelper.CreateSqlParameter("@FirstName", this.FirstName));
            pl.Add(DatabaseHelper.CreateSqlParameter("@LastName", this.LastName));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Email", this.Email));
            pl.Add(DatabaseHelper.CreateSqlParameter("@Phone", this.Phone));

            return pl;
        }

        public bool Update()
        {
            string sql = @"UPDATE Contacts SET   
                             SourceID = @SourceID
                            ,Type = @Type
                            ,FirstName = @FirstName
                            ,LastName = @LastName
                            ,Email = @Email
                            ,Phone = @Phone";

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

        public static bool DeleteBySource(int id)
        {
            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("ID", id));
            int r = DatabaseHelper.ExecuteNonQuery("DELETE FROM Contacts WHERE SourceID = @ID", pl);

            return true;
        }

        public static List<ContactModel> GetBySource(int id)
        {
            List<ContactModel> list = new List<ContactModel>();

            var pl = new List<MySqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@ID", id));
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Contacts WHERE SourceID = @ID", pl);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ContactModel(row));
            }

            return list;
        }
    }
}