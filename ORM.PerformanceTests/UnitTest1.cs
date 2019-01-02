using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ORM.PerformanceTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Profiler.ExecuteFunction(async () => await Task.Delay(1000), "Test");
        }
    }
}
