using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;
using WebApplication1;
using WebApplication1.Models;

[Authorize]
public class ClientController : Controller
{
    private int Size_Of_Page = 24;

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
                || x.Country.Value.ToUpper().Contains(search.ToUpper())).ToList();
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
        var model = new ClientModel();
        model.LoadLists();
        return View(model);
    }

    [HttpPost]
    public ActionResult Add(ClientModel client)
    {
        client.LoadLists();

        try
        {
            if (ModelState.IsValid)
            {
                if (!client.Add())
                {
                    ViewBag.Message = "Unable to add";
                    return View(client);
                }
                return RedirectToAction("Index");
            }

            return View(client);
        }
        catch(Exception e)
        {
            ViewBag.Message = e.Message;
            return View(client);
        }
    }

    public ActionResult Edit(int id)
    {
        return View(ClientModel.GetByID(id));
    }

    [HttpPost]
    public ActionResult Edit(int id, ClientModel obj)
    {
        try
        {
            obj.LoadLists();

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
        catch(Exception e)
        {
            ViewBag.Message = e.Message;
            return View(obj);
        }
    }

    public ActionResult Delete(int id)
    {
        ClientModel model = ClientModel.GetByID(id);

        try
        {
            if (!model.Delete())
            {
                ViewBag.Message = "Unable to delete";
                return View("Edit", model);
            }
            return RedirectToAction("Index");

        }
        catch(Exception e)
        {
            ViewBag.Message = e.Message;
            return View("Edit", model);
        }
    }

    public ActionResult AddContact(string count)
    {
        return PartialView("~/Views/Shared/EditorTemplates/ContactModel.cshtml", new ContactModel() { FirstName = "NewContact" + count });
    }


}