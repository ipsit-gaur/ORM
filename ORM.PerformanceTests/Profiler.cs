using Newtonsoft.Json;
using System;
using System.IO;

namespace ORM.PerformanceTests
{
    public class Profiler
    {
        private Profiler() { }

        private static ActionExecutor _actionExecutor = new ActionExecutor();

        public static void ExecuteFunction(Action action, string name)
        {
            var time = _actionExecutor.Time(action);

            File.WriteAllText(@"D:\Logs.json", JsonConvert.SerializeObject(new TestResult
            {
                Name = name,
                Ticks = time.TotalMilliseconds
            }));
        }
    }
}
