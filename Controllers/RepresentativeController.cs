using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;
using WebApplication1;
using WebApplication1.Models;

[Authorize]
public class RepresentativeController : Controller
{
    private int Size_Of_Page = 24;

    [OutputCache(Duration = 10, VaryByParam = "*")]
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

        var reps = RepresentativeModel.Get();

        if (!String.IsNullOrEmpty(search))
        {
            reps = reps.Where(x => x.Company.ToUpper().Contains(search.ToUpper())
                || x.Country.Value.ToUpper().Contains(search.ToUpper())).ToList();
        }
        switch (sortOrder)
        {
            case "Company":
                if (sortDirection == Globals.Ascending)
                    reps = reps.OrderBy(x => x.Company).ToList();
                else
                    reps = reps.OrderByDescending(x => x.Company).ToList();
                break;
            case "Country":
                if (sortDirection == Globals.Ascending)
                    reps = reps.OrderBy(x => x.Country).ToList();
                else
                    reps = reps.OrderByDescending(x => x.Country).ToList();
                break;
            default:
                reps = reps.OrderBy(stu => stu.Company).ToList();
                break;
        }

        int No_Of_Page = (pageNo ?? 1);
        return View(reps.ToPagedList(No_Of_Page, Size_Of_Page));
    }
  
    public ActionResult Add()
    {
        var model = new RepresentativeModel();
        model.LoadLists();
        return View(model);
    }

    [HttpPost]
    public ActionResult Add(RepresentativeModel rep)
    {
        rep.LoadLists();

        try
        {
            if (ModelState.IsValid)
            {
                if (!rep.Add())
                {
                    ViewBag.Message = "Unable to add";
                    return View(rep);
                }
                return RedirectToAction("Index");
            }

            return View(rep);
        }
        catch(Exception e)
        {
            ViewBag.Message = e.Message;
            return View(rep);
        }
    }

    [OutputCache(Duration = 10, VaryByParam = "*")]
    public ActionResult Edit(int id)
    {
        return View(RepresentativeModel.GetByID(id));
    }

    [HttpPost]
    public ActionResult Edit(int id, RepresentativeModel obj)
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
        RepresentativeModel model = RepresentativeModel.GetByID(id);

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