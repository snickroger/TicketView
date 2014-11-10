using TicketView;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Collections.Specialized;

namespace TicketView.Tests
{
    
    
    /// <summary>
    ///This is a test class for SharedMethodsTest and is intended
    ///to contain all SharedMethodsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SharedMethodsTest
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
        public void GetBasicAuthorizationHeaderTest()
        {
            NameValueCollection actual = SharedMethods.GetBasicAuthorizationHeader();
            Assert.IsTrue(actual.Count == 1);
            Assert.IsTrue(actual.Keys[0] == "Authorization");
            Assert.IsTrue(actual[0].StartsWith("Basic "));
        }

        [TestMethod()]
        [HostType("ASP.NET")]
        [UrlToTest("http://tickets.nick/")]
        public void GetBearerAuthorizationHeaderTest()
        {
            NameValueCollection actual = SharedMethods.GetBearerAuthorizationHeader("");
            Assert.IsTrue(actual.Count == 1);
            Assert.IsTrue(actual.Keys[0] == "Authorization");
            Assert.IsTrue(actual[0].StartsWith("Bearer "));
        }

    }
}
