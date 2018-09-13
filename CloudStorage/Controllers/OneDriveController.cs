using Newtonsoft.Json.Linq;
using OneDriveAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace CloudStorage.Controllers
{
    public class OneDriveController : Controller
    {
        // GET: OneDrive
        //public ActionResult Index()
        //{
        //    // Get file name
        //    string uploadedFileName = System.Web.HttpContext.Current.Session["uploadedFileName"] as String;

        //    // Upload file to Google Drive
        //    //GoogleDriveFilesRepository.UploadFileToGoogleDrive(uploadedFileName);

        //    return RedirectToAction("GetOneDriveFiles");
        //}



        /// <summary>
        /// clientId of you office 365 application, you can find it in https://apps.dev.microsoft.com/
        /// </summary>
        private const string ClientId = "294963ea-bf22-47d8-9ad8-8bed61b121b6";
        /// <summary>
        /// Password/Public Key of you office 365 application, you can find it in https://apps.dev.microsoft.com/
        /// </summary>
        private const string Secret = "gclknvGDK21?%nYYXA755_:";
        /// <summary>
        /// Authentication callback url, you can set it in https://apps.dev.microsoft.com/
        /// </summary>
        private const string CallbackUri = "http://localhost:63880/OneDrive/OnAuthComplate";

        /// <summary>
        /// OfficeAccessSession object in session
        /// </summary>
        public O365RestSession OfficeAccessSession
        {
            get
            {
                var officeAccess = Session["OfficeAccess"];
                if (officeAccess == null)
                {
                    officeAccess = new O365RestSession(ClientId, Secret, CallbackUri);
                    Session["OfficeAccess"] = officeAccess;
                }
                return officeAccess as O365RestSession;
            }
        }

        public ActionResult Index()
        {
            //if user is not login, redirect to office 365 for authenticate
            if (string.IsNullOrEmpty(OfficeAccessSession.AccessCode))
            {
                string url = OfficeAccessSession.GetLoginUrl("onedrive.readwrite");

                return new RedirectResult(url);
            }


            return View();
        }

        //when user complate authenticate, will be call back this url with a code
        public async Task<RedirectResult> OnAuthComplate(string code)
        {
            await OfficeAccessSession.RedeemTokensAsync(code);

            return new RedirectResult("Index");
        }

        [HttpPost]
        public async Task<ActionResult> UploadFileAndGetShareUri(HttpPostedFileBase file)
        {
            //save upload file to temp dir in local disk
            var path = Path.GetTempFileName();
            file.SaveAs(path);

            //upload the file to oneDrive and get a file id
            string oneDrivePath = file.FileName;
            //OfficeAccessSession.
            string result = await OfficeAccessSession.UploadFileAsync(path, oneDrivePath);

            JObject jo = JObject.Parse(result);
            string fileId = jo.SelectToken("id").Value<string>();

            //request oneDrive REST API with this file id to get a share link
            string shareLink = await OfficeAccessSession.GetShareLinkAsync(fileId, OneDriveShareLinkType.embed, OneDrevShareScopeType.anonymous);

            ViewBag.ShareLink = shareLink;

            

            return View();
        }
    }
}


