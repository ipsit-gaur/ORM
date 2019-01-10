using System.Collections.Generic;

namespace ORM.PerformanceTests
{
    internal class TestResult
    {
        public string Name { get; set; }
        public List<TimeResult> Results { get; set; }
    }

    internal class TimeResult
    {
        public int BuildNumber { get; set; }
        public double Ticks { get; set; }
    }
}
