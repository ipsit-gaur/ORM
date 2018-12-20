using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ORM.Tests.Data
{
    [TestClass]
    public class ContextTests
    {
        private TestContext _testContext;

        [TestInitialize]
        public void TestInit()
        {
            _testContext = new TestContext();
        }

        [TestMethod]
        public void ContextCreationTest()
        {
            Assert.IsNotNull(_testContext);
        }

        [TestMethod]
        public void ContextDbSetsInitializedTest()
        {
            Assert.IsNotNull(_testContext.Errors);
        }
    }
}
