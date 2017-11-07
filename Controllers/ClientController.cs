using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

public class ClientController : Controller
{

    // GET: Employee/GetAllEmpDetails    
    public ActionResult Index()
    {
        ModelState.Clear();
        return View(ClientModel.Get());
    }
    // GET: Employee/AddEmployee    
    public ActionResult Add()
    {
        return View();
    }

    // POST: Employee/AddEmployee    
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

    // GET: Employee/EditEmpDetails/5    
    public ActionResult Edit(int id)
    {
        return View(ClientModel.Get().Find(x => x.ID == id));
    }

    // POST: Employee/EditEmpDetails/5    
    [HttpPost]
    public ActionResult Edit(int id, ClientModel obj)
    {
        try
        {
            //EmpRepository EmpRepo = new EmpRepository();

            //EmpRepo.UpdateEmployee(obj);




            return RedirectToAction("GetAllEmpDetails");
        }
        catch
        {
            return View();
        }
    }

    // GET: Employee/DeleteEmp/5    
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