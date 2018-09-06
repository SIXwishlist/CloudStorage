using FluentFTP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace CloudStorage.Models
{
    public class FtpServerFilesRepository
    {
        public static FtpClient GetFtpClient()
        {
            /**
             * WILL IMPLEMENT
             * HARD CODE JUST FOR TESTING
             */

            // Get ftp host and credential info from DB, 
            //FtpServer ftpServer = null; 

            // Set Ftp host and credential info
            //string host = ftpServer.Address;
            //string username = ftpServer.Username;
            //string password = ftpServer.Password;

            // connect to the FTP server
            FtpClient client = new FtpClient();
            client.Host = "jacchriswang.sharefileftp.com";  
            client.Credentials = new System.Net.NetworkCredential("jacChrisWang/WJING.CA@GMAIL.COM", "dalian2$maritimE");

            return client;
        }

        public static void UploadFileToFtpServer(string fileName)
        {
            // Get FtpClient
            FtpClient client = GetFtpClient();

            // Set file path (our server)
            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/CloudStorageFiles"),
            fileName);

            try
            {
                // Connect to FTP Server
                client.Connect();

                // Check if the folder has been existing
                bool isFolderExists = client.DirectoryExists("/" + Globals.CLOUD_EVATEL_FOLDER + "/");

                // When folder is not existing, create a new folder
                if (!isFolderExists)
                {
                    client.CreateDirectory(Globals.CLOUD_EVATEL_FOLDER);
                }

                // Set working direcotry
                client.SetWorkingDirectory("/" + Globals.CLOUD_EVATEL_FOLDER + "/");

                // upload a file
                bool result = client.UploadFile(path, fileName);
  
            }
            catch (Exception ex)
            {
                //ViewBag.ErrorMessage = Globals.FTP_CONNECTION_ERROR + "\n" + ex.Message;
                //return View("FtpServerForm");

                // Error handle will be done
            }

        }

        public static List<CloudStorageFile> GetFtpServerFiles()
        {
            // Get FtpClient
            FtpClient client = GetFtpClient();

            // Ftp server File list
            List<CloudStorageFile> fileList = new List<CloudStorageFile>();

            try
            {
                // Connect to Ftp Server
                client.Connect();

                // Check if the folder has been exsiting
                bool isFolderExists = client.DirectoryExists("/" + Globals.CLOUD_EVATEL_FOLDER + "/");
                

                // When folder is existing
                if (isFolderExists)
                {
                    //var result = client.GetListing(Globals.CLOUD_EVATEL_FOLDER);
                    // Set working directory
                    client.SetWorkingDirectory("/" + Globals.CLOUD_EVATEL_FOLDER + "/");
                    var result = client.GetListing();

                    //Console.WriteLine("");
                    foreach (var file in result)
                    {

                        CloudStorageFile cloudFile = new CloudStorageFile
                        {
                            //Id = file.Id,
                            Name = file.Name,
                            Size = file.Size,
                            //Version = file.Version,
                            CreatedTime = file.Modified
                            //Link = file.
                        };
                        fileList.Add(cloudFile);
                    }

                }

     
            }
            catch (Exception ex)
            {
                // ERROR HANDLE WILL BE DONE
                //ViewBag.ErrorMessage = Globals.FTP_CONNECTION_ERROR + "\n" + ex.Message;
                //return View("FtpServerForm");
            }

            // Sort by file created time
            return fileList.OrderByDescending(f => f.CreatedTime ).ToList();
        }


        public static bool FtpConnectionTest(FtpServer ftpServer)
        {
            // Get Ftp host and credential info
            string host = ftpServer.Host;
            string username = ftpServer.Username;
            string password = ftpServer.Password;

            // Create a new ftp client
            FtpClient client = new FtpClient();
            client.Host = host;
            client.Credentials = new System.Net.NetworkCredential(username, password);

            // connect to the FTP server
            try
            {
                client.Connect();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            
        }
    }
}