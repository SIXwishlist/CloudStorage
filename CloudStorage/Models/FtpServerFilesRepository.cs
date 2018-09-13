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
        // Context declaration
        private static CloudStorageContext _context = new CloudStorageContext();

        // ONLY FOR TEST
        public static FtpClient GetFtpClient()
        {

            // Intiailize a FTP server
            FtpClient client = new FtpClient();
            client.Host = "jacchriswang.sharefileftp.com";  
            client.Credentials = new System.Net.NetworkCredential("jacChrisWang/WJING.CA@GMAIL.COM", "dalian2$maritimE");

            return client;
        }

        public static FtpClient GetFtpClientByUserId(int userId)
        {
            // Get FTP info by user id
            FtpServer ftpServer = UserRepository.GetFtpServerByUserId(userId);

            // Intiailize a FTP server
            FtpClient client = new FtpClient();
            client.Host = ftpServer.Host;
            client.Credentials = new System.Net.NetworkCredential(ftpServer.Username, ftpServer.Password);

            //client.Host = "jacchriswang.sharefileftp.com";  
            //client.Credentials = new System.Net.NetworkCredential("jacChrisWang/WJING.CA@GMAIL.COM", "dalian2$maritimE");

            return client;
        }

        public static void UploadFilesToFtpServer(int userId, FileInfo[] files)
        {
            // Get FtpClient
            FtpClient client = GetFtpClientByUserId(userId);

            

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

                

            }
            catch (Exception ex)
            {
                //ViewBag.ErrorMessage = Globals.FTP_CONNECTION_ERROR + "\n" + ex.Message;
                //return View("FtpServerForm");

                // Error handle will be done
                Debug.WriteLine("FTP connection error, " + ex.Message);
            }


            // Loop and upload all files to FTP Server
            foreach (FileInfo file in files)
            {
                try
                {
                    // upload a file
                    bool uploadedResult = client.UploadFile(file.FullName, file.Name);

                    // Set file processing result
                    FileProcessingResult result = new FileProcessingResult();
                    result.UserId = userId;
                    result.FileName = file.Name;
                    result.CloudStorageType = (byte)User.ServiceType.FtpServer;
                    result.TimeStamp = DateTime.Now;

                    // Upload successfully
                    if (uploadedResult)
                    {
                        result.IsSuccessful = true;
                    }
                    // Upload failed
                    else
                    {
                        result.IsSuccessful = false;
                    }

                    // Add result to context
                    _context.FilesProcessingResults.Add(result);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Upload error, " + ex.Message);
                }

            }

            // Save to Database;
            _context.SaveChanges();

            client.Dispose();


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