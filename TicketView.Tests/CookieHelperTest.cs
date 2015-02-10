using System.Collections.Generic;
using System.Web;

namespace TicketView.Tests
{
    class CookieHelperTest : ICookie
    {
        private Dictionary<string, HttpCookie> _cookies = new Dictionary<string, HttpCookie>();
        public HttpCookie GetCookie(HttpRequestBase request, string name)
        {
            if (!_cookies.ContainsKey(name))
                return null;
            return _cookies[name];
        }

        public void SetCookie(HttpResponseBase response, HttpCookie cookie)
        {
            _cookies.Add(cookie.Name, cookie);
        }
    }
}
