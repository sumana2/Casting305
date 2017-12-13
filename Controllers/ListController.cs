using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;
using WebApplication1;
using WebApplication1.Models;

[Authorize]
public class ListController : Controller
{
    [HttpPost]
    public JsonResult AddListOption(string value, string list)
    {
        var listItem = new ListItemModel();
        listItem.Value = value;
        listItem.List = (ListItemModel.Lists)Enum.Parse(typeof(ListItemModel.Lists), list);

        string newValue = string.Empty;

        if (listItem.Add())
        {
            newValue = value;
        }
       
        return Json(newValue);
    }
}