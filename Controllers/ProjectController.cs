using DocumentFormat.OpenXml.Packaging;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using WebApplication1;
using WebApplication1.Helpers;
using WebApplication1.Models;

[Authorize]
public class ProjectController : Controller
{
    private int Size_Of_Page = 25;

    [OutputCache(Duration = int.MaxValue, Location = OutputCacheLocation.Server, VaryByParam = "*")]
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
        var model = new ProjectModel();
        model.LoadLists();
        return View(model);
    }

    [HttpPost]
    public ActionResult Add(ProjectModel project)
    {
        project.LoadLists();

        try
        {
            if (ModelState.IsValid)
            {
                if (!project.Add())
                {
                    ViewBag.Message = "Unable to add";
                    return View(project);
                }
                Response.RemoveOutputCacheItem(Url.Action("Index", "Project"));
                return RedirectToAction("Index");
            }

            return View(project);
        }
        catch (Exception e)
        {
            ViewBag.Message = e.Message;
            return View(project);
        }
    }

    [OutputCache(Duration = int.MaxValue, VaryByParam = "*")]
    public ActionResult Edit(int id)
    {
        return View(ProjectModel.GetByID(id));
    }

    [HttpPost]
    public ActionResult Edit(int id, ProjectModel obj)
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
                Response.RemoveOutputCacheItem(Url.Action("Edit", "Project"));
                Response.RemoveOutputCacheItem(Url.Action("Index", "Project"));
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
        ProjectModel model = ProjectModel.GetByID(id);

        try
        {
            if (!model.Delete())
            {
                ViewBag.Message = "Unable to delete";
                return View("Edit", model);
            }
            Response.RemoveOutputCacheItem(Url.Action("Index", "Project"));
            return RedirectToAction("Index");

        }
        catch (Exception e)
        {
            ViewBag.Message = e.Message;
            return View("Edit", model);
        }
    }

    public ActionResult AddRole(string count)
    {
        var role = new ProjectRoleModel() { Name = "NewRole" + count };
        role.LoadLists();
        return PartialView("~/Views/Shared/EditorTemplates/ProjectRoleModel.cshtml", role);
    }

    [OutputCache(Duration = int.MaxValue, Location = OutputCacheLocation.Server, VaryByParam = "*")]
    public ActionResult RoleTalent(int id, bool? searchMode, string search, string filterValue, int? pageNo)
    {
        Globals.CachedRoleTalentIds.Add(id.ToString());

        ProjectRoleModel model;
        ViewBag.SearchMode = searchMode.HasValue && searchMode.Value;

        if (search != null)
        {
            pageNo = 1;
        }
        else
        {
            search = filterValue;
        }

        ViewBag.FilterValue = search;

        if (ViewBag.SearchMode)
        {
            model = ProjectRoleModel.GetByID(id, true);
        }
        else
        {
            model = ProjectRoleModel.GetByID(id);
        }

        if (!String.IsNullOrEmpty(search))
        {
            var searchObj = JsonConvert.DeserializeObject<SearchModel>(search);

            model.Talent = model.Talent.Where(x =>
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
        model.TalentPagedList = model.Talent.OrderBy(x => x.LastName).ToPagedList(No_Of_Page, Size_Of_Page);

        return View(model);
    }

    public JsonResult AddTalent(int projectID, int projectRoleID, int talentID)
    {
        ProjectRoleModel.AddTalent(projectRoleID, talentID);
        Response.RemoveOutputCacheItem(Url.Action("RoleTalent", "Project", new { id = projectRoleID }));
        Response.RemoveOutputCacheItem(Url.Action("Edit", "Project", new { id = projectID }));
        return Json("");
    }

    public JsonResult RemoveTalent(int projectID, int projectRoleID, int talentID)
    {
        ProjectRoleModel.RemoveTalent(projectRoleID, talentID);
        Response.RemoveOutputCacheItem(Url.Action("RoleTalent", "Project", new { id = projectRoleID }));
        Response.RemoveOutputCacheItem(Url.Action("Edit", "Project", new { id = projectID }));
        return Json("");
    }

    public ActionResult GetPresentation(int id, string actionType)
    {
        ActionResult result = null;

        var project = ProjectModel.GetByID(id);
        var roles = new List<ProjectRoleModel>();

        foreach (var role in project.Roles)
        {
            roles.Add(ProjectRoleModel.GetByID(role.ID));
        }

        try
        {
            switch (actionType)
            {
                case "GetZip":
                    result = GetZip(project);
                    break;
                case "GetSlides":
                    result = GetSlides(project);
                    break;
                case "GetPrintout":
                    result = GetPrintout(project);
                    break;
                default:
                    ViewBag.Message = string.Format("Invalid action: {0}", actionType);
                    break;
            }
        }
        catch (Exception ex)
        {
            ViewBag.Message = ex.Message;
        }

        if (result == null)
        {
            result = View("Edit", project);
        }

        return result;
    }

    private ActionResult GetZip(ProjectModel project)
    {
        string root = Server.MapPath(string.Format("/Zips/{0}", project.Title));
        if (Directory.Exists(root))
        {
            Directory.Delete(root,true);
        }
        Directory.CreateDirectory(root);

        foreach (ProjectRoleModel role in project.Roles)
        {
            string roleFolder = Path.Combine(root, role.Name);
            Directory.CreateDirectory(roleFolder);
            role.Talent = TalentModel.GetByProjectRoleID(role.ID);

            foreach (TalentModel talent in role.Talent)
            {
                string talentFolder = Path.Combine(roleFolder, string.Format("{0} {1}", talent.FirstName, talent.LastName));
                Directory.CreateDirectory(talentFolder);
                if (!string.IsNullOrEmpty(talent.ProfilePicture))
                {
                    System.IO.File.Copy(Server.MapPath(talent.ProfilePicture), Path.Combine(talentFolder, "Profile" + Path.GetExtension(talent.ProfilePicture)));
                }

                talent.GetImages();
                if (!string.IsNullOrEmpty(talent.BookPictures))
                {
                    string[] pics = talent.BookPictures.Split(',');
                    for (int i = 0; i < Math.Min(pics.Length, 10); i++)
                    {
                        System.IO.File.Copy(Server.MapPath(pics[i]), Path.Combine(talentFolder, "picture" + (i + 1) + Path.GetExtension(pics[i])));
                    }
                }
            }
        }

        string zip = Server.MapPath("/Zips/") + project.Title + ".zip";
        System.IO.File.Delete(zip);
        System.IO.Compression.ZipFile.CreateFromDirectory(root, zip, System.IO.Compression.CompressionLevel.Fastest, false, new ZipEncoder());

        try
        {
            Directory.Delete(root, true);
        }
        catch (Exception e) { }

        return File(zip, "application/zip", project.Title + ".zip");
    }

    private ActionResult GetSlides(ProjectModel project)
    {
        DirectoryInfo dir = new DirectoryInfo(Server.MapPath("/Templates"));

        var files = dir.GetFiles("Presentation1.pptx");

        files[0].CopyTo(Server.MapPath("/Templates/PresentationCopy.pptx"), true);

        using (PresentationDocument presentationDocument = PresentationDocument.Open(Server.MapPath("/Templates/PresentationCopy.pptx"), true))
        {
            int slideNo = 1;

            foreach (var role in project.Roles)
            {
                PowerPointHelper.InsertNewSlide(presentationDocument, slideNo++, string.Format("Role: {0}", role.Name), "", "", "", Server.MapPath("/Templates"));

                if (role.TalentCount > 0)
                {
                    role.Talent = TalentModel.GetByProjectRoleID(role.ID);
                }

                foreach (var talent in role.Talent)
                {
                    string name = string.Format("{0} {1}  ", talent.FirstName, talent.LastName);

                    string details = string.Format("Height: {0} Bust: {1} Waist: {2} Hip: {3} Shoe: {4}", 
                        talent.Height, talent.BustSize, talent.WaistSize, talent.HipSize, talent.ShoeSize);

                    talent.GetImages();

                    string imagesStr = string.Join(",", talent.ProfilePicture ?? string.Empty, talent.BookPictures ?? string.Empty);

                    if (!string.IsNullOrEmpty(imagesStr))
                    {
                        List<string> images = imagesStr.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                        using (LayoutHelper layout = new LayoutHelper(images, 0, 3))
                        {
                            layout.ComputeFixedPartition();
                            PowerPointHelper.InsertNewSlide(presentationDocument, slideNo++, "", name, details, "", Server.MapPath("/Templates"), layout);
                        }                        
                    }
                }
            }
        }

        return File(Server.MapPath("/Templates/PresentationCopy.pptx"), "application/vnd.openxmlformats-officedocument.presentationml.presentation", project.Title + ".pptx");
    }

    public ActionResult GetPrintout(ProjectModel project)
    {
        foreach (var role in project.Roles)
        {
            if (role.TalentCount > 0)
            {
                role.Talent = TalentModel.GetByProjectRoleID(role.ID);
            }
        }

        return View("Printout", project);
    }
}