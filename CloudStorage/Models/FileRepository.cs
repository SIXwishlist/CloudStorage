using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using CloudStorage.Models;
using System.Diagnostics;

namespace CloudStorage.Models
{
    public class FileRepository
    {
        // Context declaration
        private static CloudStorageContext _context = new CloudStorageContext();

        //file Upload to the Server.
        public static string UploadFileToServer(HttpPostedFileBase file)
        {
            // No file was chosen
            if (file == null)
            {
                return Globals.NO_CHOOSE_FILE;
            }

            // Get file size
            int fileSize = file.ContentLength;

            // File is smaller than 1k
            if (fileSize < 1024)
            {
                return Globals.FILE_TOO_SMALL;
            }

            // File is bigger than 4M
            if (fileSize > 4194304)
            {
                return Globals.FILE_TOO_BIG;
            }


            // Get timestamp
            string currentTimeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff"); ;

            // Set new file name
            string newFileName = currentTimeStamp + "_" + Path.GetFileName(file.FileName);

            // Save file name to session 
            System.Web.HttpContext.Current.Session["uploadedFileName"] = newFileName;

            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/CloudStorageFiles"),
            newFileName);

            file.SaveAs(path);


            return null;
        }

        public static void UploadFilesToCloud()
        {
            //  Fetch all users who registered Cloud service  
            List<User> userList = UserRepository.GetAllUsersWithCloudServie();

            foreach (User user in userList)
            {
                // Get file path
                // The following could not be used in background job
                //string path = HttpContext.Current.Server.MapPath("~/CloudStorageFiles/" + user.Id);

                // Get running directory
                string runningDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                string path = runningDirectory + "CloudStorageFiles\\" + user.Id;

                // Get file list from the above path
                DirectoryInfo info = new DirectoryInfo(path);
                FileInfo[] files = info.GetFiles().OrderBy(f => f.CreationTime).ToArray();

                //// Get all files
                //var fileNames = (from f in files select new { f.Name, f.FullName }).ToArray();

                // Upload file to could storage as per user's choose
                switch (user.CloudServiceType)
                {
                    // Google Drive
                    case (byte)CloudStorage.Models.User.ServiceType.GoogleDrive:
                        GoogleDriveFilesRepository.UploadFilesToGoogleDriveFolder(user.Id, files);
                        break;
                    // One Drive
                    case (byte)CloudStorage.Models.User.ServiceType.OneDrive:

                        break;
                    // Ftp Server
                    case (byte)CloudStorage.Models.User.ServiceType.FtpServer:
                        FtpServerFilesRepository.UploadFilesToFtpServer(user.Id, files);
                        break;
                    default:
                        break;
                }
                

            }
        }

        // Get process result for a specific date 
        public static List<FileProcessingResult> GetProcessResults(DateTime date)
        {
            DateTime endTime = date.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            DateTime startTime = date.Date.AddHours(0).AddMinutes(0).AddSeconds(0);

            return _context.FilesProcessingResults.Where(r =>
                r.TimeStamp <= endTime &&
                r.TimeStamp >= startTime).ToList();
            
        }
    }
}