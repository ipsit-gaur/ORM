using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORM.SQL;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ORM.Tests.Data
{
    [TestClass]
    public class ContextTests
    {
        [TestMethod]
        public void ContextCreationTest()
        {
            TestContext myContext = new TestContext();
        }

        [TestMethod]
        public void VerifyDataSetsAreInitialized()
        {
            var myContext = new TestContext();
            var error = myContext.Errors.Filter(x => x.ErrorID == 100000).Read();
            Assert.AreEqual(1, error.Count);
        }
    }
}
