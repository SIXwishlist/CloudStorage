using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudStorage.Models;
using Google.Apis.Drive.v3;

namespace CloudStorage.Controllers
{
    
    public class UserController : Controller
    {
        

        // GET: User
        public ActionResult Index()
        {
            return View("Login");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {
            // Check user login info
            User userIdDB = UserRepository.Login(user);

            // Login successfully
            if (userIdDB != null)
            {
                System.Web.HttpContext.Current.Session["UserId"] = userIdDB.Id;
                return View("AskCloudService", userIdDB);
            }
            
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            // Register user and return user id
            int userId = UserRepository.Register(user);

            // Registeration successful
            if (userId > 0)
            {
                System.Web.HttpContext.Current.Session["UserId"] = userId;
                return View("AskCloudService", user);
            }
            // registeration failed
            else
            {
                return View();
            }


        }


        public ActionResult ChooseCloudStorage(string choose)
        {
            if (choose.Equals("Yes"))
            {
                return View("ChooseCloudStorage");
            } 
            else
            {
                return Content("Do not register Cloud Service");
            }
            
        }

        public ActionResult GoogleDriveleAuthentication()
        {
            // Get Google anthentication
            DriveService service = GoogleDriveFilesRepository.RegisterService();

            if (service == null)
            {
                return Content("Authentication ERROR");
            }

            // Get current user id from session
            int userId = (int?)System.Web.HttpContext.Current.Session["UserId"] ?? 0;
            //// ERROR HANDLE NEEDED WHEN session does not exist

            // Update cloud service flag in user table 
            UserRepository.UpdateCloudServiceStatus(userId, true, (byte)CloudStorage.Models.User.ServiceType.GoogleDrive);
     
            return Content("Google Drive Service Authentication SUCCESSFULLY!");
        }

        public ActionResult OneDriveleAuthentication()
        {

            return View();
        }

        public ActionResult FtpServerAuthentication()
        {
            return View("FtpServerForm");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveFtpServer(FtpServer ftpServer)
        {
            // Ftp Server connection test
            bool result = FtpServerFilesRepository.FtpConnectionTest(ftpServer);

            if (result)
            {
                // Get current user id from session
                int userId = (int?)System.Web.HttpContext.Current.Session["UserId"] ?? 0;
                //// ERROR HANDLE NEEDED WHEN session does not exist
                ///

                // Set user id
                ftpServer.UserId = userId;

                // Update Ftp Server info in FtpServers table
                UserRepository.UpdateFtpServer(ftpServer);

                // Update Cloud Service Status in Users table
                UserRepository.UpdateCloudServiceStatus(userId, true, (byte)CloudStorage.Models.User.ServiceType.FtpServer);

                return Content("Ftp Server service has been registered SUCCESSFULLY."); 
            }
            else
            {
                return View("FtpServerForm");
            }

            
           
        }

        public ActionResult UnregisterCloudService(string choose)
        {
            if (choose.Equals("Yes"))
            {
                // Get current user id from session
                int userId = (int?)System.Web.HttpContext.Current.Session["UserId"] ?? 0;
                //// ERROR HANDLE NEEDED WHEN session does not exist
                ///
                UserRepository.UpdateCloudServiceStatus(userId, false, 0);

                return Content("Your cloud service has been cancelled.");
            }
            else
            {
                return Content("Great, you did not cancel your cloud service.");
            }
            
        }
    }
}

