using System;
using System.Diagnostics;

namespace ORM.PerformanceTests
{
    class ActionExecutor
    {
        internal TimeSpan Time(Action toTime)
        {
            var timer = Stopwatch.StartNew();
            toTime();
            timer.Stop();
            return timer.Elapsed;
        }
    }
}
