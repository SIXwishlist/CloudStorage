using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudStorage.Models;

namespace CloudStorage.Controllers
{
    public class GoogleDriveController : Controller
    {
        // GET: GoogleDrive
        public ActionResult Index()
        {
            // Get file name
            string uploadedFileName = System.Web.HttpContext.Current.Session["uploadedFileName"] as String;

            // Upload file to Google Drive
            GoogleDriveFilesRepository.UploadFileToGoogleDrive(uploadedFileName);

            return RedirectToAction("GetGoogleDriveFiles");
        }

        [HttpGet]
        public ActionResult GetGoogleDriveFiles()
        {
            return View(GoogleDriveFilesRepository.GetDriveFilesFromFolder());
        }

        [HttpPost]
        public ActionResult DeleteFile(CloudStorageFile file)
        {
            GoogleDriveFilesRepository.DeleteFile(file);
            return RedirectToAction("GetGoogleDriveFiles");
        }

        //[HttpGet]
        //public ActionResult GetGoogleDriveFiles()
        //{
        //    return View(GoogleDriveFilesRepository.GetDriveFiles());
        //}
    }
}