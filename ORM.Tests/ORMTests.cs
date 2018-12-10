using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            Assert.AreEqual(0, myContext.People.ToList().Count);
        }
    }
}
