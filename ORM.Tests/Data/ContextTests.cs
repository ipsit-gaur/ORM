using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

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
        public void DbTableFilterTest()
        {
            var myContext = new TestContext();
            var testID = 100000;
            var error = myContext.Errors.Filter(x => x.ErrorID == 100000).Read();
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(error.FirstOrDefault().ErrorID, testID);
        }

        [TestMethod]
        public void DbTableMultilpleFilterTest()
        {
            var myContext = new TestContext();
            var error = myContext.Errors
                .Filter(x => x.ErrorID == 100000)
                .Filter(y => y.Description != "abc").Read();
            Assert.AreEqual(1, error.Count);
        }
    }
}
