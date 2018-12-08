using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ORM.Tests
{
    [TestClass]
    public class BasicTest
    {
        [TestMethod]
        public void TestAccessOfProperties()
        {
            MyContext myContext = new MyContext();
            var data = myContext.People.ToList();
        }
    }
}
