﻿using Newtonsoft.Json;
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
            var items = new List<string>();
            items.Add("");

            var pl = new List<SqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@List", this.List));

            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM dbo.Lists WITH (NOLOCK) WHERE List = @List", pl);

            foreach (DataRow row in dt.Rows)
            {
                items.Add(Convert.ToString(row["Text"]));
            }

            this.ListItems = new SelectList(items);
        }

        public bool Add()
        {
            string sql = @"INSERT INTO [dbo].[Lists]([Text],[List]) VALUES (@Text,@List)";

            var pl = new List<SqlParameter>();
            pl.Add(DatabaseHelper.CreateSqlParameter("@Text", this.Value));
            pl.Add(DatabaseHelper.CreateSqlParameter("@List", this.List.ToString()));
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
    }
}