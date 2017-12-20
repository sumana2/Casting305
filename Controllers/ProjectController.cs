using DocumentFormat.OpenXml.Packaging;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebApplication1;
using WebApplication1.Helpers;
using WebApplication1.Models;

[Authorize]
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
        return View(new ProjectModel());
    }

    [HttpPost]
    public ActionResult Add(ProjectModel project)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (!project.Add())
                {
                    ViewBag.Message = "Unable to add";
                    return View(project);
                }
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

    public ActionResult Edit(int id)
    {
        return View(ProjectModel.GetByID(id));
    }

    [HttpPost]
    public ActionResult Edit(int id, ProjectModel obj)
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

    public ActionResult AddRole(string count)
    {
        return PartialView("~/Views/Shared/EditorTemplates/ProjectRoleModel.cshtml", new ProjectRoleModel() { Name = "NewRole" + count });
    }

    public ActionResult RoleTalent(int id, bool? searchMode, int? pageNo)
    {
        ProjectRoleModel model;
        ViewBag.SearchMode = searchMode.HasValue && searchMode.Value;

        if (ViewBag.SearchMode)
        {
            model = ProjectRoleModel.GetByID(id, true);
        }
        else
        {
            model = ProjectRoleModel.GetByID(id);
        }

        int No_Of_Page = (pageNo ?? 1);
        model.TalentPagedList = model.Talent.ToPagedList(No_Of_Page, Size_Of_Page);

        return View(model);
    }

    public JsonResult AddTalent(int projectRoleID, int talentID)
    {
        ProjectRoleModel.AddTalent(projectRoleID, talentID);
        return Json("");
    }

    public JsonResult RemoveTalent(int projectRoleID, int talentID)
    {
        ProjectRoleModel.RemoveTalent(projectRoleID, talentID);
        return Json("");
    }

    public ActionResult GetPresentation(int id)
    {
        var project = ProjectModel.GetByID(id);
        var roles = new List<ProjectRoleModel>();

        foreach (var role in project.Roles)
        {
            roles.Add(ProjectRoleModel.GetByID(role.ID));
        }

        DirectoryInfo dir = new DirectoryInfo(Server.MapPath("/Templates"));
        
        var files = dir.GetFiles("Presentation1.pptx");

        files[0].CopyTo(Server.MapPath("/Templates/PresentationCopy.pptx"), true);

        using (PresentationDocument presentationDocument = PresentationDocument.Open(Server.MapPath("/Templates/PresentationCopy.pptx"), true))
        {
            int slideNo = 2;

            foreach (var role in roles)
            {
                InsertSlide.InsertNewSlide(presentationDocument, slideNo++, role.Name, "", "", Server.MapPath("/Templates"));

                foreach (var talent in role.Talent)
                {
                    string info = string.Format("{1} {2}{0}Mide: {3}{0}Pesa: {4}{0}Pelo: {5}", Environment.NewLine, talent.FirstName, talent.LastName, talent.Height, talent.WaistSize, talent.HairColor);
                    InsertSlide.InsertNewSlide(presentationDocument, slideNo++, "", info, "", Server.MapPath("/Templates"));

                    InsertSlide.InsertNewSlide(presentationDocument, slideNo++, "", "", talent.ProfilePicture, Server.MapPath("/Templates"));

                    //var images = talent.GetImages();

                    //foreach (var image in images)
                    //{
                    //    InsertSlide.InsertNewSlide(presentationDocument, ++slideNo, "", "", image, Server.MapPath("/Templates"));
                    //}
                }
            }
        }

        return File(Server.MapPath("/Templates/PresentationCopy.pptx"), "application/vnd.openxmlformats-officedocument.presentationml.presentation");
    }
}