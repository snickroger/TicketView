using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using TicketView.Models;

namespace TicketView.Controllers
{
    public class AssemblaController : Controller
    {
        private string ReauthorizeToken(HttpCookie cookie)
        {
            if (cookie != null)
                return cookie.Value;

            if (Request.Cookies["RefreshToken"] != null)
            {
                string tokenUrl = String.Format(Secrets.RefreshTokenUrl, Request.Cookies["RefreshToken"].Value);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tokenUrl);
                request.Headers.Add(SharedMethods.GetBasicAuthorizationHeader());
                request.UserAgent = "Nick's Ticket View/1.0";
                request.ContentLength = 0;
                request.Method = "POST";

                string result;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }

                JObject token = JObject.Parse(result);
                string newToken = token["access_token"].Value<string>();
                Response.Cookies.Add(new HttpCookie("AuthToken", newToken) { Expires = DateTime.Now.AddSeconds(590) });

                return newToken;
            }

            throw new UnauthorizedAccessException();
        }

        [HttpGet]
        public ActionResult Milestones()
        {
            List<Milestone> milestones;
            try
            {
                JArray milestonesResponse = GetMilestones();
                milestones = ParseMilestones(milestonesResponse);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Home");
            }
            return Json(milestones, JsonRequestBehavior.AllowGet);
        }

        private JArray GetMilestones()
        {
            string url = String.Format("https://api.assembla.com/v1/spaces/{0}/milestones.json", Secrets.SpaceId);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add(SharedMethods.GetBearerAuthorizationHeader(ReauthorizeToken(Request.Cookies["AuthToken"])));
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            string result;
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                result = sr.ReadToEnd();
            }

            return JArray.Parse(result);
        }

        private static List<Milestone> ParseMilestones(JArray milestonesResponse)
        {
            return (from JObject jo in milestonesResponse select new Milestone(jo)).ToList();
        }

        [HttpGet]
        public ActionResult Tickets(string id)
        {
            List<Ticket> tickets;
            try
            {
                JArray ticketsResponse = GetTickets("7260603");
                tickets = ParseTickets(ticketsResponse);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Home");
            }
            return Json(tickets, JsonRequestBehavior.AllowGet);
        }

        private JArray GetTickets(string milestoneId)
        {
            int page = 1;
            JArray tickets = new JArray();
            while (page <= 6)
            {
                string url = String.Format("https://api.assembla.com/v1/spaces/{0}/tickets/milestone/{1}.json?per_page=25&page={2}&ticket_status=all", Secrets.SpaceId, milestoneId, page++);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Headers.Add(SharedMethods.GetBearerAuthorizationHeader(ReauthorizeToken(Request.Cookies["AuthToken"])));
                request.Method = "GET";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string result;
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }

                if (String.IsNullOrWhiteSpace(result))
                    break;

                JArray t = JArray.Parse(result);
                foreach (JObject jo in t.Where(a => a["assigned_to_id"].Value<string>() == Secrets.UserId))
                {
                    tickets.Add(jo);
                }
            }
            return tickets;
        }

        private static List<Ticket> ParseTickets(JArray ticketsResponse)
        {
            return (from JObject jo in ticketsResponse select new Ticket(jo)).ToList();
        }

    }
}
