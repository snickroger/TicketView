﻿using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web;
using System.Web.Routing;
using Newtonsoft.Json.Linq;
using TicketView.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Web.Mvc;
using TicketView.Models;

namespace TicketView.Tests
{
    
    
    /// <summary>
    ///This is a test class for HomeControllerTest and is intended
    ///to contain all HomeControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HomeControllerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod()]
        [HostType("ASP.NET")]
        [UrlToTest("http://tickets.nick")]
        public void LoginTestNoCode()
        {
            HomeController target = new HomeController();
            target.Cookies = new CookieHelperTest();
            ActionResult result = target.Login();

            Assert.IsInstanceOfType(result, typeof(RedirectResult));
        }

        [TestMethod()]
        [HostType("ASP.NET")]
        [UrlToTest("http://tickets.nick/")]
        [DeploymentItem("TicketView.dll")]
        public void ReauthorizeTokenTestWithAuthToken()
        {
            HomeController_Accessor target = new HomeController_Accessor();
            HttpCookie cookie = new HttpCookie("AuthToken", "123");
            string expected = "123";
            string actual;

            actual = target.ReauthorizeToken(cookie);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        [HostType("ASP.NET")]
        [UrlToTest("http://tickets.nick/")]
        [DeploymentItem("TicketView.dll")]
        public void ReauthorizeTokenTestWithoutAuthToken()
        {
            HomeController_Accessor target = new HomeController_Accessor();
            target.Cookies = new CookieHelperTest();
            try
            {
                target.ReauthorizeToken(null);
                Assert.Fail();
            }
            catch (UnauthorizedAccessException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod()]
        [HostType("ASP.NET")]
        [UrlToTest("http://tickets.nick/")]
        [DeploymentItem("TicketView.dll")]
        public void ParseMilestonesTest()
        {
            string sampleResponse = System.IO.File.ReadAllText(@"..\..\SampleResponses\GetMilestones.txt");
            JArray milestonesResponse = JArray.Parse(sampleResponse);
            List<Milestone> actual = HomeController_Accessor.ParseMilestones(milestonesResponse);
            Assert.IsTrue(actual.Count > 0);
        }

        [TestMethod()]
        [HostType("ASP.NET")]
        [UrlToTest("http://tickets.nick/")]
        [DeploymentItem("TicketView.dll")]
        public void ParseTicketsTest()
        {
            string sampleResponse = System.IO.File.ReadAllText(@"..\..\SampleResponses\GetTickets.txt");
            JArray ticketsResponse = JArray.Parse(sampleResponse);
            List<Ticket> actual = HomeController_Accessor.ParseTickets(ticketsResponse);
            Assert.IsTrue(actual.Count > 0);
        }

    }
}
