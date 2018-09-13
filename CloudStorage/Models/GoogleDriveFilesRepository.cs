using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;

namespace CloudStorage.Models
{
    public class GoogleDriveFilesRepository
    {
        // Context declaration
        private static CloudStorageContext _context = new CloudStorageContext();

        // Scope declaration
        public static string[] Scopes = { DriveService.Scope.Drive };

        //create Drive API service.
        public static DriveService GetService()
        {
            //get Credentials from client_secret.json file 
            UserCredential credential;

            // Get running directory
            string runningDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

            using (var stream = new FileStream(runningDirectory + "credentials.json", FileMode.Open, FileAccess.Read))
            {
                //String FolderPath = @"D:\";
                //String FilePath = Path.Combine(FolderPath, "DriveServiceCredentials.json");
                string credPath = runningDirectory + "DriveServiceCredentials.json";

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            //create Drive API service.
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoogleDriveRestAPI-v3",
            });
            return service;
        }

        // Get google drive files from folder
        public static List<CloudStorageFile> GetDriveFilesFromFolder()
        {
            DriveService service = GetService();

            // Get Folder ID for Evatel sound records
            string folderId = GetGoogleDriveFolderId(Globals.CLOUD_EVATEL_FOLDER, service);
            List<CloudStorageFile> fileList = new List<CloudStorageFile>();
            string pageToken = null;

            do
            {
                var request = service.Files.List();
                request.Q = "'" + folderId + "' in parents";
                request.Spaces = "drive";
                request.Fields = "nextPageToken, files(id, name, size, version, createdTime, webContentLink)";
                request.PageToken = pageToken;
                var result = request.Execute();
                foreach (var file in result.Files)
                {

                    CloudStorageFile googleFile = new CloudStorageFile
                    {
                        Id = file.Id,
                        Name = file.Name,
                        Size = file.Size,
                        Version = file.Version,
                        CreatedTime = file.CreatedTime,
                        Link = file.WebContentLink
                    };
                    fileList.Add(googleFile);
                }

                pageToken = result.NextPageToken;
            } while (pageToken != null);

            return fileList;

        }    

        //get all files from Google Drive.
        public static List<CloudStorageFile> GetDriveFiles()
        {
            DriveService service = GetService();

            // define parameters of request.
            FilesResource.ListRequest FileListRequest = service.Files.List();

            //listRequest.PageSize = 10;
            //listRequest.PageToken = 10;
            FileListRequest.Fields = "nextPageToken, files(id, name, size, version, createdTime)";

            //get file list.
            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            List<CloudStorageFile> FileList = new List<CloudStorageFile>();

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    CloudStorageFile File = new CloudStorageFile
                    {
                        Id = file.Id,
                        Name = file.Name,
                        Size = file.Size,
                        Version = file.Version,
                        CreatedTime = file.CreatedTime
                    };
                    FileList.Add(File);
                }
            }
            return FileList;
        }

        // Get goodle drive Folder ID
        public static string GetGoogleDriveFolderId(string folderName, DriveService service)
        {
            //DriveService service = GetService();

            // Get Folder ID for Evatel sound records
            string folderId = null;
            string pageToken = null;
            do
            {
                var request = service.Files.List();
                request.Q = "mimeType='application/vnd.google-apps.folder' and name='" + Globals.CLOUD_EVATEL_FOLDER + "'";
                request.Spaces = "drive";
                request.Fields = "nextPageToken, files(id, name)";
                request.PageToken = pageToken;
                var result = request.Execute();
                foreach (var file in result.Files)
                {
                    if (file.Name.Equals(Globals.CLOUD_EVATEL_FOLDER))
                    {
                        folderId = file.Id;
                        break;
                    }
                }
                pageToken = result.NextPageToken;
            } while (pageToken != null);

            return folderId;
        }

        public static string CreateFolderOnGoogleDriver(string folderName, DriveService service)
        {
            //DriveService service = GetService();

            Google.Apis.Drive.v3.Data.File fileMeta = new Google.Apis.Drive.v3.Data.File();
            fileMeta.Name = folderName;
            fileMeta.MimeType = "application/vnd.google-apps.folder";

            Google.Apis.Drive.v3.FilesResource.CreateRequest request;

            request = service.Files.Create(fileMeta);
            request.Fields = "id";

            var file = request.Execute();

            return file.Id;
        }

        //file Upload to the Google Drive.
        public static void UploadFileToGoogleDrive(string fileName)
        {

            DriveService service = GetService();

            // Get folder id of evatel sound records 
            string folderId = GetGoogleDriveFolderId(Globals.CLOUD_EVATEL_FOLDER, service);

            // Folder has not been existing yet
            if (folderId == null)
            {
                // Create a Evatel folder, and return folder id
                folderId = CreateFolderOnGoogleDriver(Globals.CLOUD_EVATEL_FOLDER, service);
            }

            // Save folderId to session
            //HttpContext.Current.Session["folderId"] = folderId;

            // Upload file to the folder           
            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/CloudStorageFiles"),
            fileName);

            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName,
                MimeType = MimeMapping.GetMimeMapping(path),
                Parents = new List<string>
                    {
                        folderId
                    }
            };


            FilesResource.CreateMediaUpload request;

            using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
                request.Fields = "id";
                request.Upload();
            }


            // Clear session
            //System.Web.HttpContext.Current.Session.Remove("uploadedFileName");

            var uploadedfile = request.ResponseBody;
            /**
             * going to check if uploading is successful
             */

        }


        public static void UploadFilesToGoogleDriveFolder(int userId, FileInfo[] files)
        {

            DriveService service = RegisterServiceByUserId(userId);

            // Get folder id of evatel sound records 
            string folderId = GetGoogleDriveFolderId(Globals.CLOUD_EVATEL_FOLDER, service);

            // Folder has not been existing yet
            if (folderId == null)
            {
                // Create a Evatel folder, and return folder id
                folderId = CreateFolderOnGoogleDriver(Globals.CLOUD_EVATEL_FOLDER, service);
            }

            // Loop files to upload all files
            foreach (FileInfo file in files)
            {
                
                var FileMetaData = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = file.Name,
                    MimeType = MimeMapping.GetMimeMapping(file.FullName),
                    Parents = new List<string>
                    {
                        folderId
                    }
                };


                FilesResource.CreateMediaUpload request;

                using (var stream = new System.IO.FileStream(file.FullName, System.IO.FileMode.Open))
                {
                    request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
                    request.Fields = "id";
                    request.Upload();
                }


                // Clear session
                //System.Web.HttpContext.Current.Session.Remove("uploadedFileName");

                // Check upload result
                var uploadedfile = request.ResponseBody;

                // Set file processing result
                FileProcessingResult result = new FileProcessingResult();
                result.UserId = userId;
                result.FileName = file.Name;
                result.CloudStorageType = (byte)User.ServiceType.GoogleDrive;
                result.TimeStamp = DateTime.Now;

                // Upload successfully
                if (uploadedfile.Id != null)
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

            // Save to Database;
            _context.SaveChanges();

            service.Dispose();

        }


        // file save to server path
        private static void SaveStream(MemoryStream stream, string FilePath)
        {
            using (System.IO.FileStream file = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                stream.WriteTo(file);
            }
        }

        //Delete file from the Google drive
        public static void DeleteFile(CloudStorageFile files)
        {
            DriveService service = GetService();
            try
            {
                // Initial validation.
                if (service == null)
                    throw new ArgumentNullException("service");

                if (files == null)
                    throw new ArgumentNullException(files.Id);

                // Make the request.
                service.Files.Delete(files.Id).Execute();
            }
            catch (Exception ex)
            {
                throw new Exception("Request Files.Delete failed.", ex);
            }
        }

        //create Drive API service.  
        //////// USERD FOR STEP 2
        public static DriveService RegisterServiceByUserId(int userId)
        {
            //get Credentials from client_secret.json file 
            UserCredential credential;

            // Get running directory
            string runningDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            
            //string credentialJson = 

            using (var stream = new FileStream(runningDirectory + "credentials.json", FileMode.Open, FileAccess.Read))
            {

                //string credPath = runningDirectory + "UserCredentials\\" + "ServiceCredentials.json" + "." + userId;
                // The following coluld not be used on the background job 
                //string credPath = HttpContext.Current.Server.MapPath("~/UserCredentials/" + userId);
                string credPath = runningDirectory + "UserCredentials\\" + userId;

                try
                {
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                                        GoogleClientSecrets.Load(stream).Secrets,
                                        Scopes,
                                        "user",
                                        CancellationToken.None,
                                        new FileDataStore(credPath, true)).Result;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
                
            }

            //create Drive API service.
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoogleDriveRestAPI-v3",
            });
            return service;
        }
    }
}