using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using WebApplication1.Helpers;
using static WebApplication1.Models.ListItemModel;

namespace WebApplication1.Models
{
    public class BaseModel
    {
        public void LoadLists()
        {
            PropertyInfo[] properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in properties)
            {
                if (p.PropertyType == typeof(ListItemModel) && p.CanWrite && p.CanRead)
                {
                    MethodInfo mget = p.GetGetMethod(false);
                    MethodInfo mset = p.GetSetMethod(false);

                    if (p.GetValue(this) == null)
                    {
                        p.SetValue(this, new ListItemModel((ListItemModel.Lists)Enum.Parse(typeof(ListItemModel.Lists), p.Name)));
                    }

                    var model = (ListItemModel)p.GetValue(this);

                    if (model.List == Lists.None)
                    {
                        model.List = (ListItemModel.Lists)Enum.Parse(typeof(ListItemModel.Lists), p.Name);
                    }

                    model.LoadList();
                }
            }
        }
    }
}