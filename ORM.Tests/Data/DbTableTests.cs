using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ORM.Tests.Data
{
    [TestClass]
    public class DbTableTests
    {
        private TestContext _testContext;

        [TestInitialize]
        public void TestInit()
        {
            _testContext = new TestContext();
        }

        [TestMethod]
        public void DbTableFilterTest()
        {
            var testID = 100000;
            var error = _testContext.Errors.Filter(x => x.ErrorID == 100000).Read();
            Assert.AreEqual(1, error.Count);
            Assert.AreEqual(error.FirstOrDefault().ErrorID, testID);
        }

        [TestMethod]
        public void DbTableMultilpleFilterTest()
        {
            var error = _testContext.Errors
                .Filter(x => x.ErrorID == 100000)
                .Filter(y => y.Description != "abc").Read();
            Assert.AreEqual(1, error.Count);
        }
    }
}
