using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace CloudStorage.Models
{
    public class FileRepository
    {
        //file Upload to the Server.
        public static string FileUploadToServer(HttpPostedFileBase file)
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
    }
}