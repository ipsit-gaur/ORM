using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ORM.Tests.Data
{
    [TestClass]
    public class ContextTests
    {
        private TestDbContext _testContext;

        [TestInitialize]
        public void TestInit()
        {
            _testContext = new TestDbContext();
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
