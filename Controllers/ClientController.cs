using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using PagedList;

public class ClientController : Controller
{  
    public ActionResult Index(string sortOrder, int? Page_No)
    {
        ViewBag.SortingName = String.IsNullOrEmpty(sortOrder) ? "Company" : "";
        ViewBag.SortingDate = sortOrder == "Date_Enroll" ? "Country" : "";

        var clients = ClientModel.Get();
        
        switch (sortOrder)
        {
            case "Company":
                clients = clients.OrderByDescending(x => x.Company).ToList();
                break;
            case "Country":
                clients = clients.OrderBy(x => x.Country).ToList();
                break;
            default:
                clients = clients.OrderBy(stu => stu.Company).ToList();
                break;
        }

        ModelState.Clear();

        int Size_Of_Page = 4;
        int No_Of_Page = (Page_No ?? 1);
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
                if (client.Add())
                {
                    ViewBag.Message = "Employee details added successfully";
                }
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
            obj.Update();

            return RedirectToAction("Index");
        }
        catch(Exception e)
        {
            ViewBag.Message = "Error!";
            return View();
        }
    }

    public ActionResult Delete(int id)
    {
        try
        {
            ClientModel c = new ClientModel() { ID = id };
            if (c.Delete())
            {
                //ViewBag.AlertMsg = "Employee details deleted successfully";
            }
            return RedirectToAction("Index");

        }
        catch
        {
            return View();
        }
    }


}