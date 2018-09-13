/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Web;

namespace OneDriveFinal.Controllers
{

    //[Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public void Register(string userId)
        {

            //try
            //{

            //    // Initialize the GraphServiceClient.
            //    GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();


            //}
            //catch (ServiceException se)
            //{
            //    if (se.Error.Message == Resource.Error_AuthChallengeNeeded) return new EmptyResult();
            //    return RedirectToAction("Index", "Error", new { message = string.Format(Resource.Error_Message, Request.RawUrl, se.Error.Code, se.Error.Message) });
            //}

            // Save current User to session 
            System.Web.HttpContext.Current.Session["currentUserId"] = userId;
            //return RedirectToAction("SignIn", "Account");
            HttpContext.GetOwinContext().Authentication.Challenge(
              new AuthenticationProperties { RedirectUri = "/" },
              OpenIdConnectAuthenticationDefaults.AuthenticationType);




            //return Content("register SUCCESSFULLY");

        }


        public ActionResult ShowMessage(string message)
        {
            ViewBag.Message = message;
            return View();
        }   
  
    }

}