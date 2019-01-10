using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

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

            var text = string.Empty;
            var maxBuild = 0;
            var result = new List<TestResult>();
            try
            {
                text = File.ReadAllText(_logsPath);
                result = JsonConvert.DeserializeObject<List<TestResult>>(text);
                maxBuild = result?.FirstOrDefault(x => x.Name == name)?.Results?.Max(x => x.BuildNumber) ?? 0;
            }
            catch (FileNotFoundException ex)
            {
                File.Create(_logsPath);
            }
            result = result ?? new List<TestResult>();
            var test = result.FirstOrDefault(x => x.Name == name);
            if (test == null)
            {
                test = new TestResult { Name = name, Results = new List<TimeResult>() };
                result.Add(test);
            }
            test.Results.Add(new TimeResult
            {
                BuildNumber = maxBuild + 1,
                Ticks = time.TotalMilliseconds
            });
            if (result != null)
                File.WriteAllText(_logsPath, JsonConvert.SerializeObject(result));
        }
    }
}
