using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1;
using WebApplication1.Models;

public class TalentController : Controller
{
    private int Size_Of_Page = 12;

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
        return View(TalentModel.GetByID(id));
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

    public JsonResult AddPhoto()
    {
        string thumbnailUrl = string.Empty;

        try
        {
            var files = HttpContext.Request.Files;

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file.ContentLength > 0)
                {
                    // Create storagecredentials object by reading the values from the configuration (appsettings.json)
                    var storageCredentials = new StorageCredentials("casting305", "TNDeEXG40fZg7FQuGT1MyFue / 4AXHTzsCi0OiQeQrMv17xENp + BO7fvrv / G6HyJ3Lz2P1cVzpAvYtmQVCYv7bQ ==");
                    
                    // Create cloudstorage account by passing the storagecredentials
                    var storageAccount = new CloudStorageAccount(storageCredentials, true);

                    // Create blob client
                    var blobClient = storageAccount.CreateCloudBlobClient();

                    // Get reference to the container
                    var container = blobClient.GetContainerReference("images");

                    // Retrieve reference to a blob named "myblob".
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.FileName);

                    // Create or overwrite the "myblob" blob with contents from a local file.
                    blockBlob.UploadFromStream(file.InputStream);

                    thumbnailUrl = blockBlob.Uri.ToString();
                }
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }

        return Json(thumbnailUrl);
    }
}