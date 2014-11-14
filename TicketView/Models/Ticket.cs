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
        public string Component { get; private set; }
        private int _priority;

        public Ticket(JObject jo)
        {
            Id = jo["id"].Value<string>();
            Number = jo["number"].Value<int>();
            Created = jo["created_on"].Value<DateTime>();
            Title = jo["summary"].Value<string>();
            Status = jo["status"].Value<string>();
            Component = jo["custom_fields"]["Component"].Value<string>();
            _priority = jo["priority"].Value<int>();
        }

        public string PanelStyle { get
        {
            switch (Status)
            {
                case "Fixed":
                case "Certified":
                    return "panel-success";
                case "Test":
                    return "panel-info";
                case "Invalid":
                    return "panel-danger";
                case "Review":
                    return "panel-warning";
                default:
                    return "panel-primary";
            }
        }}

        public bool PanelFaded { get { return PanelStyle != "panel-primary" || Status == "Coded"; }}

        public string Priority
        {
            get { switch (_priority)
            {
                case 1:
                    return "Highest";
                case 2:
                    return "High";
                case 3:
                    return "Normal";
                case 4:
                    return "Low";
                case 5:
                default:
                    return "Lowest";
            } }
        }
    }
}