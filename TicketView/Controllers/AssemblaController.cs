﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace TicketView.Controllers
{
    public class AssemblaController : ApiController
    {
        public void GetMilestones()
        {
            // https://api.assembla.com/v1/spaces/ccWk4o9Bqr4jfCacwqjQWU/milestones.xml
        }
    }
}
