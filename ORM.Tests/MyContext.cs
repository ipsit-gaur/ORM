using ORM.Data;

namespace ORM.Tests
{
    public class MyContext : DataContext
    {
        public ODataSet<string> People { get; set; }

        public MyContext() : base("")
        {

        }
    }
}
