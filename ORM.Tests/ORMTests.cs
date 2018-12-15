using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORM.Extensions;
using System.Linq;

namespace ORM.Tests
{
    [TestClass]
    public class ORMTests
    {
        [TestMethod]
        public void ContextCreationTest()
        {
            MyContext myContext = new MyContext();
        }

        [TestMethod]
        public void VerifyDataSetsAreInitialized()
        {
            var myContext = new MyContext();
            Assert.AreEqual(8, myContext.Categories.Filter(x => x.CategoryID == 1).Read().Count);
        }
    }
}
