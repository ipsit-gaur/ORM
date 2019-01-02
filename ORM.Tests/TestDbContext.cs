using ORM.Data;

namespace ORM.Tests
{
    public class TestDbContext : DataContext
    {
        public DbTable<Errors> Errors { get; set; }
        public DbTable<Categories> Categories { get; set; }
        public DbTable<Currency> Currencies { get; set; }

        public TestDbContext() : base("")
        {

        }
    }
}
