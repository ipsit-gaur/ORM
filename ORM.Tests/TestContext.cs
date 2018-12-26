using ORM.Data;

namespace ORM.Tests
{
    public class TestContext : DataContext
    {
        public DbTable<Errors> Errors { get; set; }
        public DbTable<Categories> Categories { get; set; }

        public TestContext() : base("")
        {

        }
    }
}
