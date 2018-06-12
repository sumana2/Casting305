using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using PagedList;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;
using Newtonsoft.Json;

[Authorize]
public class TalentController : Controller
{
    private int Size_Of_Page = 25;

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

        var talent = TalentModel.Get();

        if (!String.IsNullOrEmpty(search))
        {
            var searchObj = JsonConvert.DeserializeObject<SearchModel>(search);

            talent = talent.Where(x =>
                   (searchObj.FirstName == null || x.FirstName.ToUpper().Contains(searchObj.FirstName.ToUpper()))
                && (searchObj.LastName == null || x.LastName.ToUpper().Contains(searchObj.LastName.ToUpper()))
                && (searchObj.Height == null || x.Height.ToUpper().Contains(searchObj.Height.ToUpper()))
                && (searchObj.BustSize == null || x.BustSize.ToUpper().Contains(searchObj.BustSize.ToUpper()))
                && (searchObj.WaistSize == null || x.WaistSize.ToUpper().Contains(searchObj.WaistSize.ToUpper()))
                && (searchObj.HipSize == null || x.HipSize.ToUpper().Contains(searchObj.HipSize.ToUpper()))
                && (searchObj.ShoeSize == null || x.ShoeSize.ToUpper().Contains(searchObj.ShoeSize.ToUpper()))
                && (searchObj.RepName == null || x.RepDisplayName.ToUpper().Contains(searchObj.RepName.ToUpper()))
                && (searchObj.DateOfBirth == null || (x.DateOfBirth.HasValue && x.DateOfBirth.Value.ToShortDateString().Contains(searchObj.DateOfBirth)))
                && (searchObj.Country == null || x.Country.Value.ToUpper().Contains(searchObj.Country.ToUpper()))
                && (searchObj.EyeColor == null || x.EyeColor.Value.ToUpper().Contains(searchObj.EyeColor.ToUpper()))
                && (searchObj.HairColor == null || x.HairColor.Value.ToUpper().Contains(searchObj.HairColor.ToUpper()))
                && (searchObj.Ethnicity == null || x.Ethnicity.Value.ToUpper().Contains(searchObj.Ethnicity.ToUpper()))
                && (searchObj.Talent == null || x.Talent.Value.ToUpper().Contains(searchObj.Talent.ToUpper()))
                && (searchObj.Gender == null || x.Gender.Value.ToUpper().Equals(searchObj.Gender.ToUpper()))
                ).ToList();
        }

        int No_Of_Page = (pageNo ?? 1);
        return View(talent.OrderBy(x => x.LastName).ToPagedList(No_Of_Page, Size_Of_Page));
    }
  
    public ActionResult Add()
    {
        var model = new TalentModel();
        model.LoadLists();
        return View(model);
    }

    [HttpPost]
    public ActionResult Add(TalentModel model)
    {
        model.LoadLists();

        try
        {
            if (ModelState.IsValid)
            {
                if (!model.Add())
                {
                    ViewBag.Message = "Unable to add";
                    return View(model);
                }
                return RedirectToAction("Index");
            }

            return View(model);
        }
        catch(Exception e)
        {
            ViewBag.Message = e.Message;
            return View(model);
        }
    }

    public ActionResult Edit(int id)
    {
        return View(TalentModel.GetByID(id));
    }

    [HttpPost]
    public ActionResult Edit(int id, TalentModel obj)
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
        TalentModel model = TalentModel.GetByID(id);

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

    public JsonResult AddPhoto()
    {
        string url = string.Empty;

        try
        {
            string path = Server.MapPath("~/TalentPhotos/Temp");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var files = HttpContext.Request.Files;

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file.ContentLength > 0)
                {
                    string filename = string.Format("{0}{1}", Guid.NewGuid().ToString(), Path.GetExtension(file.FileName));
                    string filepath = Path.Combine(path, filename);
                    file.SaveAs(filepath);

                    url = string.Format("{0}{1}", "/TalentPhotos/Temp/", filename);
                }
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }

        return Json(url);
    }
}