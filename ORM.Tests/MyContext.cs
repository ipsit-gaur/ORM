using ORM.Data;

namespace ORM.Tests
{
    public class MyContext : DataContext
    {
        public ODataSet<Categories> Categories { get; set; }

        public MyContext() : base("")
        {

        }
    }
}
