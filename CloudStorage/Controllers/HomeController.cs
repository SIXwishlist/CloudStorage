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

        public ActionResult UploadFileToServer(HttpPostedFileBase file)
        {
            string result = FileRepository.UploadFileToServer(file);

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


    }

}