using ORM.Data;

namespace ORM.Tests
{
    public class MyContext : DataContext
    {
        public DbTable<Categories> Categories { get; set; }

        public MyContext() : base("")
        {

        }
    }
}
