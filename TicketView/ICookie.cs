using System.Web;

namespace TicketView
{
    public interface ICookie
    {
        HttpCookie GetCookie(HttpRequestBase request, string name);
        void SetCookie(HttpResponseBase response, HttpCookie cookie);
    }
}