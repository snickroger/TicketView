using System.Web;

namespace TicketView
{
    public class CookieHelper : ICookie
    {
        public HttpCookie GetCookie(HttpRequestBase request, string name)
        {
            if (request.Cookies[name] == null)
                return null;
            return request.Cookies[name];
        }

        public void SetCookie(HttpResponseBase response, HttpCookie cookie)
        {
            response.Cookies.Add(cookie);
        }
    }
}