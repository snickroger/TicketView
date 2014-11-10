using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace TicketView.Models
{
    public class Ticket
    {
        public string Id { get; private set; }
        public int Number { get; private set; }
        public DateTime Created { get; private set; }
        public string Title { get; private set; }
        public string Status { get; private set; }

        public Ticket(JObject jo)
        {
            Id = jo["id"].Value<string>();
            Number = jo["number"].Value<int>();
            Created = jo["created_on"].Value<DateTime>();
            Title = jo["summary"].Value<string>();
            Status = jo["status"].Value<string>();
        }

    }
}