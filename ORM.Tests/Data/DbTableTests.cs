using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ORM.Tests.Data
{
    [TestClass]
    public class DbTableTests
    {
        private TestDbContext _testContext;
        private int TestID = 1;

        [TestInitialize]
        public void TestInit()
        {
            _testContext = new TestDbContext();
        }

        [TestMethod]
        public void DbTableLoadAllTest()
        {
            var categories = _testContext.Categories.Read();
            Assert.AreEqual(8, categories.Count);
        }

        [TestMethod]
        public void DbTableFilterTest()
        {
            int testID = 1;
            var category = _testContext.Categories.Filter(x => x.CategoryID == TestID).Read();
            Assert.AreEqual(1, category.Count);
            Assert.AreEqual(category.FirstOrDefault().CategoryID, testID);
        }

        [TestMethod]
        public void DbTableMultilpleFilterTest()
        {
            var category = _testContext.Categories
                .Filter(x => x.CategoryID == 1)
                .Filter(y => y.CategoryName != "abc").Read();
            Assert.AreEqual(1, category.Count);
        }

        [TestMethod]
        public void DbTableMultilpleFilterTest2()
        {
            var category = _testContext.Categories
                .Filter(x => x.CategoryID == 1)
                .Filter(y => y.CategoryName != "Beverages").Read();
            Assert.AreEqual(0, category.Count);
        }

        [TestMethod]
        public void DbTableAddTest()
        {
            _testContext.Tests.Add(new TestTable
            {
                ID = 1,
                Text = "Demo"
            });
            _testContext.Tests.Save();
        }

        [TestMethod]
        public void DbTableUpdateTest()
        {
            var tests = _testContext.Tests.Read();
            tests.FirstOrDefault(x => x.ID == 1).Text = "Updated";
            _testContext.Tests.Update(tests.FirstOrDefault(x => x.ID == 1));
            _testContext.Tests.Save();
        }

        [TestMethod]
        public void DbTableDeleteTest()
        {
            var tests = _testContext.Tests.Read();
            _testContext.Tests.Delete(tests.FirstOrDefault(x => x.ID == 1));
            _testContext.Tests.Save();
        }
    }
}
