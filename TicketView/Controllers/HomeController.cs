using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace TicketView.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Request.Cookies["AuthToken"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Login(string code = null)
        {
            if (code == null)
            {
                string authUrl = String.Format(Secrets.AuthorizationUrl, Secrets.ApplicationId);
                return Redirect(authUrl);
            }

            string auth = String.Format("{0}:{1}", Secrets.ApplicationId, Secrets.ApplicationSecret);
            auth = String.Format("Basic {0}", Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(auth)));

            string tokenUrl = String.Format(Secrets.TokenUrl, code);

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(tokenUrl);
            request.Headers.Add("Authorization", auth);
            request.UserAgent = "Nick's Ticket View/1.0";
            request.ContentLength = 0;
            request.Method = "POST";

            string result;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }
            catch (WebException wex)
            {
                using (StreamReader sr = new StreamReader(wex.Response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }

            JObject token = JObject.Parse(result);
            if (token["access_token"] == null)
            {
                return Content(token.ToString());
            }

            Response.Cookies.Add(new HttpCookie("AuthToken", token["access_token"].Value<string>()));

            return RedirectToAction("Index");

        }

    }
}
