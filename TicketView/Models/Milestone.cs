using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace TicketView.Models
{
    public class Milestone
    {
        public string Title { get; private set; }
        public string Id { get; private set; }

        public Milestone(JObject jo)
        {
            Title = jo["title"].Value<string>();
            Id = jo["id"].Value<string>();
        }

    }
}