using ORM.Attributes;
using ORM.Data;

namespace ORM.Tests
{
    public class TestTable : DbEntity
    {
        [Key]
        public int ID { get; set; }
        public string Text { get; set; }
    }
}
