using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

                return Content("File uploaded");
            }
            else
            {
                return Content("Upload cancelled");
            }
        }
    }
}