using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using WebApplication1.Helpers;

namespace WebApplication1.Models
{
    public class ListItemModel
    {
        public string Value { get; set; }

        public Lists List { get; set; }

        public SelectList ListItems { get; set; }

        public bool AllowAdd { get { return List != Lists.Representative; } } 

        public enum Lists { None, Country, Gender, Nationality, EyeColor, HairColor, Ethnicity, Representative, ProjectType, Talent };

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
            if (List == Lists.Representative)
            {
                List<KeyValuePair<string, string>> kvp = new List<KeyValuePair<string, string>>();
                kvp.Add(new KeyValuePair<string, string>(string.Empty, string.Empty));

                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Representatives");

                foreach (DataRow row in dt.Rows)
                {
                    kvp.Add(new KeyValuePair<string, string>(Convert.ToString(row["ID"]), Convert.ToString(row["Company"])));
                }

                this.ListItems = new SelectList(kvp, "Key", "Value");
            }

            else
            {
                var items = new List<string>();
                items.Add("");

                var pl = new List<MySqlParameter>();
                pl.Add(DatabaseHelper.CreateSqlParameter("@List", this.List.ToString()));

                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Lists WHERE List = @List", pl);

                foreach (DataRow row in dt.Rows)
                {
                    items.Add(Convert.ToString(row["Text"]));
                }

                this.ListItems = new SelectList(items);
            }
        }

        public bool Add()
        {
            if (this.List != Lists.None && this.AllowAdd)
            {
                string sql = @"INSERT INTO Lists(Text,List) VALUES (@Text,@List)";

                var pl = new List<MySqlParameter>();
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

            return false;
        }

    }
}