using ORM.Data;

namespace ORM.Tests
{
    public class Errors : DbEntity
    {
        public int ErrorID { get; set; }
        public string Description { get; set; }
    }
}