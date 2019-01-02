using System;

namespace ORM.PerformanceTests
{
    public class Profiler
    {
        private Profiler() { }

        private static ActionExecutor _actionExecutor = new ActionExecutor();

        public void ExecuteFunction(Action action, string name)
        {
            var time = _actionExecutor.Time(action);
        }
    }
}
