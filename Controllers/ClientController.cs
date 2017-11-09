using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;
using WebApplication1;
using WebApplication1.Models;

public class ClientController : Controller
{
    private int Size_Of_Page = 4;

    public ActionResult Index(string sortOrder, string sortDirection, string search, string filterValue, int? pageNo)
    {
        ViewBag.CurrentSortOrder = sortOrder;
        ViewBag.CurrentsortDirection = sortDirection;
        ViewBag.SortDirection = sortDirection == Globals.Descending ? Globals.Ascending : Globals.Descending;

        if (search != null)
        {
            pageNo = 1;
        }
        else
        {
            search = filterValue;
        }

        ViewBag.FilterValue = search;

        var clients = ClientModel.Get();

        if (!String.IsNullOrEmpty(search))
        {
            clients = clients.Where(x => x.Company.ToUpper().Contains(search.ToUpper())
                || x.Country.ToUpper().Contains(search.ToUpper())).ToList();
        }
        switch (sortOrder)
        {
            case "Company":
                if (sortDirection == Globals.Ascending)
                    clients = clients.OrderBy(x => x.Company).ToList();
                else
                    clients = clients.OrderByDescending(x => x.Company).ToList();
                break;
            case "Country":
                if (sortDirection == Globals.Ascending)
                    clients = clients.OrderBy(x => x.Country).ToList();
                else
                    clients = clients.OrderByDescending(x => x.Country).ToList();
                break;
            default:
                clients = clients.OrderBy(stu => stu.Company).ToList();
                break;
        }

        int No_Of_Page = (pageNo ?? 1);
        return View(clients.ToPagedList(No_Of_Page, Size_Of_Page));
    }
  
    public ActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Add(ClientModel client)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (!client.Add())
                {
                    ViewBag.AlertMsg = "Unable to add";
                    return View();
                }
                return RedirectToAction("Index");
            }

            return View();
        }
        catch(Exception e)
        {
            ViewBag.Message = "Error!";
            return View();
        }
    }

    public ActionResult Edit(int id)
    {
        return View(ClientModel.Get().Find(x => x.ID == id));
    }

    [HttpPost]
    public ActionResult Edit(int id, ClientModel obj)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (!obj.Update())
                {
                    ViewBag.AlertMsg = "Unable to update";
                    return View();
                }
                return RedirectToAction("Index");
            }

            return View();
        }
        catch(Exception e)
        {
            ViewBag.AlertMsg = e.Message;
            return View();
        }
    }

    public ActionResult Delete(int id)
    {
        try
        {
            ClientModel c = new ClientModel() { ID = id };
            if (!c.Delete())
            {
                ViewBag.AlertMsg = "Unable to delete";
                return View();
            }
            return RedirectToAction("Index");

        }
        catch(Exception e)
        {
            ViewBag.AlertMsg = e.Message;
            return View();
        }
    }


}