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
        //defined scope.
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
            string folderId = GetGoogleDriveFolderId(Globals.CLOUD_EVATEL_FOLDER);
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



            //DriveService service = GetService();

            //// define parameters of request.
            //FilesResource.ListRequest FileListRequest = service.Files.List();

            ////listRequest.PageSize = 10;
            ////listRequest.PageToken = 10;
            //FileListRequest.Fields = "nextPageToken, files(id, name, size, version, createdTime)";

            ////get file list.
            //IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            //List<GoogleDriveFiles> FileList = new List<GoogleDriveFiles>();

            //if (files != null && files.Count > 0)
            //{
            //    foreach (var file in files)
            //    {
            //        GoogleDriveFiles File = new GoogleDriveFiles
            //        {
            //            Id = file.Id,
            //            Name = file.Name,
            //            Size = file.Size,
            //            Version = file.Version,
            //            CreatedTime = file.CreatedTime
            //        };
            //        FileList.Add(File);
            //    }
            //}
            //return FileList;
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
        public static string GetGoogleDriveFolderId(string folderName)
        {
            DriveService service = GetService();

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

        public static string CreateFolderOnGoogleDriver()
        {
            DriveService service = GetService();

            Google.Apis.Drive.v3.Data.File fileMeta = new Google.Apis.Drive.v3.Data.File();
            fileMeta.Name = Globals.CLOUD_EVATEL_FOLDER;
            fileMeta.MimeType = "application/vnd.google-apps.folder";

            Google.Apis.Drive.v3.FilesResource.CreateRequest request;

            request = service.Files.Create(fileMeta);
            request.Fields = "id";

            var file = request.Execute();

            return file.Id;
        }

        public static void UploadFileToGoogleDriveFolder(string folderId, string fileName)
        {
            DriveService service = GetService();

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

        //file Upload to the Google Drive.
        public static void UploadFileToGoogleDrive(string fileName)
        {

            DriveService service = GetService();

            // Get folder id of evatel sound records 
            string folderId = GetGoogleDriveFolderId(Globals.CLOUD_EVATEL_FOLDER);

            // Folder has not been existing yet
            if (folderId == null)
            {
                // Create a Evatel folder, and return folder id
                folderId = CreateFolderOnGoogleDriver();
            }

            // Save folderId to session
            //HttpContext.Current.Session["folderId"] = folderId;

            // Upload file to the folder
            UploadFileToGoogleDriveFolder(folderId, fileName);

        }



        ////file Upload to the Google Drive.
        //public static void FileUpload(HttpPostedFileBase file)
        //{
        //    if (file != null && file.ContentLength > 0)
        //    {
        //        DriveService service = GetService();

        //        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/CloudStorageFiles"),
        //        Path.GetFileName(file.FileName));

        //        file.SaveAs(path);

        //        var FileMetaData = new Google.Apis.Drive.v3.Data.File();
        //        FileMetaData.Name = Path.GetFileName(file.FileName);
        //        FileMetaData.MimeType = MimeMapping.GetMimeMapping(path);

        //        FilesResource.CreateMediaUpload request;

        //        using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
        //        {
        //            request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
        //            request.Fields = "id";
        //            request.Upload();
        //        }
        //    }
        //}

        //Download file from Google Drive by fileId.
        public static string DownloadGoogleFile(string fileId)
        {
            DriveService service = GetService();

            string FolderPath = System.Web.HttpContext.Current.Server.MapPath("/GoogleDriveFiles/");
            FilesResource.GetRequest request = service.Files.Get(fileId);

            string FileName = request.Execute().Name;
            string FilePath = System.IO.Path.Combine(FolderPath, FileName);

            MemoryStream stream1 = new MemoryStream();

            // Add a handler which will be notified on progress changes.
            // It will notify on each chunk download and when the
            // download is completed or failed.
            request.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case DownloadStatus.Downloading:
                        {
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                    case DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete.");
                            SaveStream(stream1, FilePath);
                            break;
                        }
                    case DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            break;
                        }
                }
            };
            request.Download(stream1);
            return FilePath;
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
        public static DriveService RegisterService()
        {
            //get Credentials from client_secret.json file 
            UserCredential credential;

            // Get running directory
            string runningDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

            using (var stream = new FileStream(runningDirectory + "credentials.json", FileMode.Open, FileAccess.Read))
            {
                // Get user id from session
                int userId = (int?)System.Web.HttpContext.Current.Session["UserId"] ?? 0;

                // ERROR HANDLE NEEDED
                if (userId <= 0 )
                {
                    return null;
                }

                string credPath = runningDirectory + "UserCredentials\\" + "ServiceCredentials.json" + "." + userId;

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