﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using TicketView.Models;

namespace TicketView.Controllers
{
    public class HomeController : Controller
    {
        public ICookie Cookies = new CookieHelper();
        [HttpGet]
        public ActionResult Index()
        {
            if (Cookies.GetCookie(Request, "AuthToken") == null)
            {
                return RedirectToAction("Login");
            }

            string selectedMilestone = "";
            JArray milestones = GetMilestones();
            if (Cookies.GetCookie(Request, "SelectedMilestone") != null)
                selectedMilestone = Cookies.GetCookie(Request, "SelectedMilestone").Value;

            // if cookie equals invalid or deleted milestone id, just take the first one
            if (milestones.Cast<JObject>().All(jo => jo["id"].Value<string>() != selectedMilestone))
                selectedMilestone = milestones.First()["id"].Value<string>();

            bool completedHidden = Cookies.GetCookie(Request, "CompletedHidden") != null && Convert.ToBoolean(Cookies.GetCookie(Request, "CompletedHidden").Value);

            ViewModel vm = new ViewModel(ParseMilestones(milestones), completedHidden, selectedMilestone, ParseTickets(GetTickets(selectedMilestone)));
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(ViewModel submitted)
        {
            Cookies.SetCookie(Response, new HttpCookie("SelectedMilestone", submitted.SelectedMilestone) { Expires = DateTime.Now.AddYears(1) });
            Cookies.SetCookie(Response, new HttpCookie("CompletedHidden", submitted.CompletedHidden.ToString()) { Expires = DateTime.Now.AddYears(1) });
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Login(string code = null)
        {
            if (code == null)
            {
                if (Cookies.GetCookie(Request, "RefreshToken") != null)
                {
                    ReauthorizeToken(null);
                    return RedirectToAction("Index");
                }
                string authUrl = String.Format(Secrets.AuthorizationUrl, Secrets.ApplicationId);
                return Redirect(authUrl);
            }

            string tokenUrl = String.Format(Secrets.TokenUrl, code);

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(tokenUrl);
            request.Headers.Add(SharedMethods.GetBasicAuthorizationHeader());
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

            Cookies.SetCookie(Response, new HttpCookie("AuthToken", token["access_token"].Value<string>()) { Expires = DateTime.Now.AddSeconds(590) });
            Cookies.SetCookie(Response, new HttpCookie("RefreshToken", token["refresh_token"].Value<string>()) { Expires = DateTime.Now.AddDays(30) });

            return RedirectToAction("Index");

        }

        private string ReauthorizeToken(HttpCookie cookie)
        {
            if (cookie != null)
                return cookie.Value;

            if (Cookies.GetCookie(Request, "RefreshToken") != null)
            {
                string tokenUrl = String.Format(Secrets.RefreshTokenUrl, Cookies.GetCookie(Request, "RefreshToken").Value);

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
                Cookies.SetCookie(Response, new HttpCookie("AuthToken", newToken) { Expires = DateTime.Now.AddSeconds(590) });

                return newToken;
            }

            throw new UnauthorizedAccessException();
        }

        private JArray GetMilestones()
        {
            string url = String.Format("https://api.assembla.com/v1/spaces/{0}/milestones.json", Secrets.SpaceId);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add(SharedMethods.GetBearerAuthorizationHeader(ReauthorizeToken(Cookies.GetCookie(Request, "AuthToken"))));
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
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

        private JArray GetTickets(string milestoneId)
        {
            int page = 1;
            JArray tickets = new JArray();
            while (page <= 10)
            {
                string url = String.Format("https://api.assembla.com/v1/spaces/{0}/tickets/milestone/{1}.json?per_page=50&page={2}&ticket_status=all", Secrets.SpaceId, milestoneId, page++);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Headers.Add(SharedMethods.GetBearerAuthorizationHeader(ReauthorizeToken(Cookies.GetCookie(Request, "AuthToken"))));
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
            return (from JObject jo in ticketsResponse select new Ticket(jo)).OrderByDescending(a=>a.Number).ToList();
        }

        public class ViewModel
        {
            private readonly List<Milestone> _milestones;
            private readonly List<Ticket> _tickets;
            public IEnumerable<SelectListItem> Milestones { get { return _milestones.Select(a => new SelectListItem() {Text = a.Title, Value = a.Id}); } }
            public List<Ticket> Tickets { get { return _tickets; } }
            public string SelectedMilestone { get; set; }
            public bool CompletedHidden { get; set; }

            public ViewModel()
            {

            }

            public ViewModel(List<Milestone> milestones, bool completedHidden, string selectedMilestone = null, List<Ticket> tickets = null)
            {
                _milestones = milestones;
                CompletedHidden = completedHidden;
                if (selectedMilestone != null)
                {
                    SelectedMilestone = selectedMilestone;
                    _tickets = tickets;
                    if (completedHidden && _tickets != null)
                        _tickets = _tickets.Where(a => !a.PanelFaded).ToList();
                }
            }
        }
    }
}
