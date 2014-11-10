using Newtonsoft.Json.Linq;
using TicketView.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using TicketView.Models;
using System.Collections.Generic;
using System.Web;

namespace TicketView.Tests
{
    
    
    /// <summary>
    ///This is a test class for AssemblaControllerTest and is intended
    ///to contain all AssemblaControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AssemblaControllerTest
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
        [UrlToTest("http://tickets.nick/")]
        [DeploymentItem("TicketView.dll")]
        public void ReauthorizeTokenTestWithAuthToken()
        {
            AssemblaController_Accessor target = new AssemblaController_Accessor();
            HttpCookie cookie = new HttpCookie("AuthToken","123");
            string expected = "123";
            string actual;

            actual = target.ReauthorizeToken(cookie);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ParseMilestones
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\nrogers\\Documents\\Visual Studio 2010\\Projects\\TicketView\\TicketView", "/")]
        [UrlToTest("http://localhost:56249/")]
        [DeploymentItem("TicketView.dll")]
        public void ParseMilestonesTest()
        {
            string sampleResponse = System.IO.File.ReadAllText(@"..\..\SampleResponses\GetMilestones.txt");
            JArray milestonesResponse = JArray.Parse(sampleResponse);
            List<Milestone> actual = AssemblaController_Accessor.ParseMilestones(milestonesResponse);
            Assert.IsTrue(actual.Count > 0);
        }
    }
}
