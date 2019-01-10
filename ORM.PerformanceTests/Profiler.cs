using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;

namespace ORM.PerformanceTests
{
    public class Profiler
    {
        private static readonly string _logsPath;
        static Profiler()
        {
            _logsPath = ConfigurationManager.AppSettings["perfTestsPath"];
        }

        private static ActionExecutor _actionExecutor = new ActionExecutor();

        public static void ExecuteFunction(Action action, string name)
        {
            var time = _actionExecutor.Time(action);

            File.WriteAllText(_logsPath, JsonConvert.SerializeObject(new TestResult
            {
                Name = name,
                Ticks = time.TotalMilliseconds
            }));
        }
    }
}
