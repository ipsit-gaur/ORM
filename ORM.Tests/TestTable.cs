using ORM.Data;

namespace ORM.Tests
{
    public class TestTable : DbEntity
    {
        public int ID { get; set; }
        public string Text { get; set; }
    }
}
