using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace TicketView
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }

    public static class SharedMethods
    {
        public static NameValueCollection GetBasicAuthorizationHeader()
        {
            NameValueCollection ret = new NameValueCollection();
            string auth = String.Format("{0}:{1}", Secrets.ApplicationId, Secrets.ApplicationSecret);
            auth = String.Format("Basic {0}", Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(auth)));

            ret.Add("Authorization", auth);
            return ret;
        }

        public static NameValueCollection GetBearerAuthorizationHeader(string token)
        {
            NameValueCollection ret = new NameValueCollection();
            
            string auth = String.Format("Bearer {0}", token);

            ret.Add("Authorization", auth);
            return ret;
        }

    }

}