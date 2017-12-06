using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using WebApplication1.Helpers;

namespace WebApplication1.Models
{
    public class ListItemModel
    {
        public string Value { get; set; }

        public Lists List { get; set; }

        public SelectList ListItems { get; set; }

        public enum Lists { Country };

        public ListItemModel()
        {

        }

        public ListItemModel(string value)
        {
            this.Value = value;
        }

        public ListItemModel(Lists list, string value)
        {
            this.List = list;
            this.Value = value;

            LoadList();
        }

        public ListItemModel(Lists list)
        {
            this.List = list;

            LoadList();
        }

        public void LoadList()
        {
            List<string> l = new List<string>();
            l.Add("");
            l.Add("USA");
            l.Add("Mexico");

            this.ListItems = new SelectList(l);
        }

        //public bool Add()
        //{
        //    string sql = @"INSERT INTO [dbo].[Clients]([Company],[Country],[Email],[Phone],[Address],[BillingInfo],[AdminEmail])
        //                   VALUES (@Company,@Country,@Email,@Phone,@Address,@BillingInfo,@AdminEmail)";

        //    var pl = new List<SqlParameter>();
        //    pl.Add(DatabaseHelper.CreateSqlParameter("@Company", this.Company));
        //    pl.Add(DatabaseHelper.CreateSqlParameter("@Country", this.Country));
        //    pl.Add(DatabaseHelper.CreateSqlParameter("@Email", this.Email));
        //    pl.Add(DatabaseHelper.CreateSqlParameter("@Phone", this.Phone));
        //    pl.Add(DatabaseHelper.CreateSqlParameter("@Address", this.Address));
        //    pl.Add(DatabaseHelper.CreateSqlParameter("@BillingInfo", this.BillingInfo));
        //    pl.Add(DatabaseHelper.CreateSqlParameter("@AdminEmail", this.AdminEmail));
        //    int r = DatabaseHelper.ExecuteNonQuery(sql, pl);

        //    if (r >= 1)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //} 

        //public static List<ListModel> Get(string listName)
        //{
        //    List<ListModel> list = new List<ListModel>();

        //    //var pl = new List<SqlParameter>();
        //    //pl.Add(DatabaseHelper.CreateSqlParameter("@ListName", listName));

        //    //DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM dbo.Lists WITH (NOLOCK) WHERE ListName = @ListName", pl);

        //    //foreach (DataRow row in dt.Rows)
        //    //{
        //    //    var listItem = new ListModel();
        //    //    listItem.Value = Convert.ToString(row["Value"]);
        //    //    listItem.Text = Convert.ToString(row["Text"]);
        //    //    listItem.ListName = Convert.ToString(row["ListName"]);
        //    //    list.Add(listItem);
        //    //}

        //    var listItem = new ListModel();
        //    listItem.Value = "USA";
        //    listItem.Text = "USA";
        //    listItem.ListName = "Country";
        //    list.Add(listItem);

        //    return list;
        //}
    }
}