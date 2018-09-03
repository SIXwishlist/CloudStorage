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
        public ActionResult UploadFileToFtpServer(FtpServer ftpServer)
        {
            //return Content(ftpServer.Address + "\n" + ftpServer.Username + "\n" + ftpServer.Password);

            string host = ftpServer.Address;
            string username = ftpServer.Username;
            string password = ftpServer.Password;
            // Get file name
            string uploadedFileName = System.Web.HttpContext.Current.Session["uploadedFileName"] as String;

            // Set our server path
            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/CloudStorageFiles"),
            uploadedFileName);

            // Set Ftp server path
            //string ftpPath = Globals.CLOUD_EVATEL_FOLDER + "/" + uploadedFileName;

            // connect to the FTP server
            FtpClient client = new FtpClient();
            client.Host = "jacchriswang.sharefileftp.com";  //host
            client.Credentials = new System.Net.NetworkCredential("jacChrisWang/WJING.CA@GMAIL.COM", "dalian2$maritimE");

            try
            {
                client.Connect();

                bool isFolderExists = client.FileExists(Globals.CLOUD_EVATEL_FOLDER);

                // When folder is not existing
                if (!isFolderExists)
                {
                    client.CreateDirectory(Globals.CLOUD_EVATEL_FOLDER);
                }

                // Set working direcotry
                client.SetWorkingDirectory("/" + Globals.CLOUD_EVATEL_FOLDER);

                // upload a file
                bool result = client.UploadFile(path, uploadedFileName);
                //return View("GetFileList");

                return RedirectToAction("GetFtpServerFiles");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = Globals.FTP_CONNECTION_ERROR + "\n" + ex.Message;
                return View("FtpServerForm");
            }

            
        }

        public ActionResult GetFtpServerFiles()
        {
            List<CloudStorageFile> fileList = new List<CloudStorageFile>();

            // connect to the FTP server
            FtpClient client = new FtpClient();
            client.Host = "jacchriswang.sharefileftp.com";  //host
            client.Credentials = new System.Net.NetworkCredential("jacChrisWang/WJING.CA@GMAIL.COM", "dalian2$maritimE");

            try
            {
                client.Connect();

                bool isFolderExists = client.FileExists(Globals.CLOUD_EVATEL_FOLDER);
                

                // When folder is existing
                if (isFolderExists)
                {
                    //var result = client.GetListing(Globals.CLOUD_EVATEL_FOLDER);
                    // Set working directory
                    client.SetWorkingDirectory("/" + Globals.CLOUD_EVATEL_FOLDER);
                    var result = client.GetListing();

                    //Console.WriteLine("");
                    foreach (var file in result)
                    {

                        CloudStorageFile googleFile = new CloudStorageFile
                        {
                            //Id = file.Id,
                            Name = file.Name,
                            Size = file.Size,
                            //Version = file.Version,
                            CreatedTime = file.Modified
                            //Link = file.
                        };
                        fileList.Add(googleFile);
                    }

                }

                // upload a file
                //bool result = client.UploadFile(path, ftpPath);
                return View("GetCloudDriveFiles", fileList);

                //return RedirectToAction("GetFtpServerFiles");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = Globals.FTP_CONNECTION_ERROR + "\n" + ex.Message;
                return View("FtpServerForm");
            }
            
        }


    }

}