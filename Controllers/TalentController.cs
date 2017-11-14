using PagedList;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1;
using WebApplication1.Models;

public class TalentController : Controller
{
    private int Size_Of_Page = 4;

    public ActionResult Index(string sortOrder, string sortDirection, string search, string filterValue, int? pageNo)
    {
        if (search != null)
        {
            pageNo = 1;
        }
        else
        {
            search = filterValue;
        }

        ViewBag.FilterValue = search;

        var clients = TalentModel.Get();

        if (!String.IsNullOrEmpty(search))
        {
            clients = clients.Where(x => x.FirstName.ToUpper().Contains(search.ToUpper())
                || x.LastName.ToUpper().Contains(search.ToUpper())).ToList();
        }

        int No_Of_Page = (pageNo ?? 1);
        return View(clients.ToPagedList(No_Of_Page, Size_Of_Page));
    }
  
    public ActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Add(TalentModel talent)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (!talent.Add())
                {
                    ViewBag.Message = "Unable to add";
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
        return View(TalentModel.Get().Find(x => x.ID == id));
    }

    [HttpPost]
    public ActionResult Edit(int id, TalentModel obj)
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
        catch(Exception e)
        {
            ViewBag.Message = e.Message;
            return View(obj);
        }
    }

    public ActionResult Delete(int id)
    {
        try
        {
            TalentModel model = new TalentModel() { ID = id };
            if (!model.Delete())
            {
                ViewBag.Message = "Unable to delete";
                return View();
            }
            return RedirectToAction("Index");

        }
        catch(Exception e)
        {
            ViewBag.Message = e.Message;
            return View();
        }
    }

    public JsonResult FileUpload()
    {
        try
        {
            var hfc = HttpContext.Request.Files;
            string path = "/content/files/contact/";

            for (int i = 0; i < hfc.Count; i++)
            {
                var hpf = hfc[i];
                if (hpf.ContentLength > 0)
                {
                    string fileName = "";
                   
                    
                    fileName = hpf.FileName;
                    
                    string fullPathWithFileName = path + fileName;
                    //hpf.SaveAs(Server.MapPath(fullPathWithFileName));
                }
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }

        //if (file != null)
        //{
        //    string pic = System.IO.Path.GetFileName(file.FileName);
        //    string path = System.IO.Path.Combine(
        //                           Server.MapPath("~/images/profile"), pic);
        //    // file is uploaded
        //    //file.SaveAs(path);

        //}

        return Json("");
        // after successfully uploading redirect the user
        //return RedirectToAction("actionname", "controller name");
    }
}