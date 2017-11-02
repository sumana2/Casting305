using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            DirectoryInfo dir = new DirectoryInfo(Server.MapPath("/Templates"));

            var files = dir.GetFiles("Presentation1.pptx");

            files[0].CopyTo(Server.MapPath("/Templates/PresentationCopy.pptx"), true);

            using (PresentationDocument presentationDocument = PresentationDocument.Open(Server.MapPath("/Templates/PresentationCopy.pptx"), true))
            {
                string c = string.Format("Lora lora {0}Mide: 30{0}Pesa: 50{0}Pelo: Rojo", Environment.NewLine);
                InsertSlide.InsertNewSlide(presentationDocument, 1, "", c, false);
                InsertSlide.InsertNewSlide(presentationDocument, 2, "", "", true);

                c = string.Format("Lora Moka {0}Mide: 50{0}Pesa: 10{0}Pelo: Negro", Environment.NewLine);
                InsertSlide.InsertNewSlide(presentationDocument, 3, "", c, false);
                InsertSlide.InsertNewSlide(presentationDocument, 4, "", "", true);
            }

            return File(Server.MapPath("/Templates/PresentationCopy.pptx"), "application/vnd.openxmlformats-officedocument.presentationml.presentation");

            //string connectionString = "Server=tcp:casting305.database.windows.net,1433;Initial Catalog=Casting305;Persist Security Info=False;User ID=dbadmin;Password=Baddbpass1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            //using (System.Data.SqlClient.SqlConnection _con = new System.Data.SqlClient.SqlConnection(connectionString))
            //{
            //    string queryStatement = "SELECT TOP 1 * FROM [SalesLT].[Customer]";

            //    using (var _cmd = new System.Data.SqlClient.SqlCommand(queryStatement, _con))
            //    {
            //        var customerTable = new System.Data.DataTable("Top5Customers");

            //        var _dap = new System.Data.SqlClient.SqlDataAdapter(_cmd);

            //        _con.Open();
            //        _dap.Fill(customerTable);
            //        _con.Close();

            //        string s = customerTable.Rows[0].ItemArray[3].ToString();

            //    }
            //}

            //List<string> thumbnailUrls = new List<string>();

            //// Create storagecredentials object by reading the values from the configuration (appsettings.json)
            //var storageCredentials = new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials("casting305", "TNDeEXG40fZg7FQuGT1MyFue / 4AXHTzsCi0OiQeQrMv17xENp + BO7fvrv / G6HyJ3Lz2P1cVzpAvYtmQVCYv7bQ ==");
            //// Create cloudstorage account by passing the storagecredentials
            //var storageAccount = new Microsoft.WindowsAzure.Storage.CloudStorageAccount(storageCredentials, true);

            //// Create blob client
            //var blobClient = storageAccount.CreateCloudBlobClient();

            //// Get reference to the container
            //var container = blobClient.GetContainerReference("images");

            //// Set the permission of the container to public
            //container.SetPermissions(new Microsoft.WindowsAzure.Storage.Blob.BlobContainerPermissions { PublicAccess = Microsoft.WindowsAzure.Storage.Blob.BlobContainerPublicAccessType.Blob });

            //Microsoft.WindowsAzure.Storage.Blob.BlobContinuationToken continuationToken = null;

            //Microsoft.WindowsAzure.Storage.Blob.BlobResultSegment resultSegment = null;

            ////Call ListBlobsSegmentedAsync and enumerate the result segment returned, while the continuation token is non-null.
            ////When the continuation token is null, the last page has been returned and execution can exit the loop.
            ////do
            ////{
            //    //This overload allows control of the page size. You can return all remaining results by passing null for the maxResults parameter,
            //    //or by calling a different overload.
            //    resultSegment = container.ListBlobsSegmented("", true, Microsoft.WindowsAzure.Storage.Blob.BlobListingDetails.All, 10, continuationToken, null, null);

            //    foreach (var blobItem in resultSegment.Results)
            //    {
            //        thumbnailUrls.Add(blobItem.StorageUri.PrimaryUri.ToString());
            //    }

            //    //Get the continuation token.
            //    continuationToken = resultSegment.ContinuationToken;
            ////}

            ////while (continuationToken != null);

            return View();
        }

        public ActionResult Clients()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}