using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudStorage.Models;

namespace CloudStorage.Controllers
{
    public class FileController : Controller
    {
        // GET: File
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Upload(string choose)
        {

            if (choose.Equals("Yes"))
            {
                // Uplaod files to Cloud storage
                FileRepository.UploadFilesToCloud();

                return Content("File uploaded");
            }
            else
            {
                return Content("Upload cancelled");
            }

        }
    }
}