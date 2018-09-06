using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudStorage.Models;
using FluentFTP;


namespace CloudStorage.Controllers
{
    public class FtpServerController : Controller
    {
        // GET: FtpServer
        public ActionResult Index()
        {
            FtpServer ftpServer = new FtpServer();
            return View("FtpServerForm");
        }

        [HttpPost]
        public ActionResult ProcessForm(FtpServer ftpServer)
        {
            // Save ftp server info into DB
            /* is going to implement */

            // Get file name
            string uploadedFileName = System.Web.HttpContext.Current.Session["uploadedFileName"] as String;

            // Upload file to FTP Server
            FtpServerFilesRepository.UploadFileToFtpServer(uploadedFileName);

            // Display file list on FTP Server
            return RedirectToAction("GetFtpServerFiles");
        }
        

        public ActionResult GetFtpServerFiles()
        {
            return View("GetCloudDriveFiles", FtpServerFilesRepository.GetFtpServerFiles()); 
            
        }


    }

}