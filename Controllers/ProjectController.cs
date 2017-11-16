using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;
using WebApplication1;
using WebApplication1.Models;

public class ProjectController : Controller
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

        var projects = ProjectModel.Get();

        if (!String.IsNullOrEmpty(search))
        {
            projects = projects.Where(x => x.Title.ToUpper().Contains(search.ToUpper())
                || x.Company.ToUpper().Contains(search.ToUpper())).ToList();
        }
        switch (sortOrder)
        {
            case "Company":
                if (sortDirection == Globals.Ascending)
                    projects = projects.OrderBy(x => x.Company).ToList();
                else
                    projects = projects.OrderByDescending(x => x.Company).ToList();
                break;
            case "Title":
                if (sortDirection == Globals.Ascending)
                    projects = projects.OrderBy(x => x.Title).ToList();
                else
                    projects = projects.OrderByDescending(x => x.Title).ToList();
                break;
            default:
                projects = projects.OrderBy(stu => stu.Company).ToList();
                break;
        }

        int No_Of_Page = (pageNo ?? 1);
        return View(projects.ToPagedList(No_Of_Page, Size_Of_Page));
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
                    ViewBag.Message = "Unable to add";
                    return View();
                }
                return RedirectToAction("Index");
            }

            return View();
        }
        catch (Exception e)
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
                    ViewBag.Message = "Unable to update";
                    return View(obj);
                }
                return RedirectToAction("Index");
            }

            return View(obj);
        }
        catch (Exception e)
        {
            ViewBag.Message = e.Message;
            return View(obj);
        }
    }

    public ActionResult Delete(int id)
    {
        try
        {
            ClientModel model = new ClientModel() { ID = id };
            if (!model.Delete())
            {
                ViewBag.Message = "Unable to delete";
                return View();
            }
            return RedirectToAction("Index");

        }
        catch (Exception e)
        {
            ViewBag.Message = e.Message;
            return View();
        }
    }


}