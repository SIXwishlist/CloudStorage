using CloudStorage.Models;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace CloudStorage.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        //[HttpGet]
        //public ActionResult GetGoogleDriveFiles()
        //{
        //    return View(GoogleDriveFilesRepository.GetDriveFiles());
        //}

        //[HttpPost]
        //public ActionResult DeleteFile(GoogleDriveFiles file)
        //{
        //    GoogleDriveFilesRepository.DeleteFile(file);
        //    return RedirectToAction("GetGoogleDriveFiles");
        //}

        //[HttpPost]
        //public ActionResult UploadFile(HttpPostedFileBase file)
        //{
        //    GoogleDriveFilesRepository.FileUpload(file);
        //    return RedirectToAction("GetGoogleDriveFiles");
        //}

        public ActionResult UploadFileToServer(HttpPostedFileBase file)
        {
            string result = FileRepository.FileUploadToServer(file);

            // Choose which cloud storage 
            if (result == null)
            {
                return View("ChooseCloudStorage");
            }
            // Return to file choose window
            else
            {
                ViewBag.ErrorMessage = result;
                return View("Index");
            }
                 

            
        }

        public void DownloadFile(string id)
        {
            string FilePath = GoogleDriveFilesRepository.DownloadGoogleFile(id);


            Response.ContentType = "application/zip";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(FilePath));
            Response.WriteFile(System.Web.HttpContext.Current.Server.MapPath("~/GoogleDriveFiles/" + Path.GetFileName(FilePath)));
            Response.End();
            Response.Flush();
        }
    }
}