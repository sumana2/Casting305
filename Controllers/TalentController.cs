﻿using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using PagedList;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;

[Authorize]
public class TalentController : Controller
{
    private int Size_Of_Page = 24;

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
            string query = search.ToUpper();

            talent = talent.Where(x => x.FirstName.ToUpper().Contains(query)
                || x.LastName.ToUpper().Contains(query)
                || x.Height.ToUpper().Contains(query)
                || x.BustSize.ToUpper().Contains(query)
                || x.WaistSize.ToUpper().Contains(query)
                || x.HipSize.ToUpper().Contains(query)
                || x.ShoeSize.ToUpper().Contains(query)
                || x.RepDisplayName.ToUpper().Contains(query)
                || (x.DateOfBirth.HasValue && x.DateOfBirth.Value.ToShortDateString().Contains(query))
                || x.Country.Value.ToUpper().Contains(query)
                || x.EyeColor.Value.ToUpper().Contains(query)
                || x.HairColor.Value.ToUpper().Contains(query)
                || x.Ethnicity.Value.ToUpper().Contains(query)
                || x.Talent.Value.ToUpper().Contains(query)).ToList();
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
    public ActionResult Add(TalentModel talent)
    {
        talent.LoadLists();

        try
        {
            if (ModelState.IsValid)
            {
                if (!talent.Add())
                {
                    ViewBag.Message = "Unable to add";
                    return View(talent);
                }
                return RedirectToAction("Index");
            }

            return View(talent);
        }
        catch(Exception e)
        {
            ViewBag.Message = e.Message;
            return View(talent);
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