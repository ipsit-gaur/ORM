using Serilog;

namespace ORM.Logger
{
    public class LogManager
    {
        public LogManager()
        {
            Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console()
                        .CreateLogger();
        }

        public void Info(string message)
        {
            Log.Logger.Information(message);
        }
    }
}
