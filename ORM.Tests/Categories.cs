using ORM.Data;

namespace ORM.Tests
{
    public class Categories : DbEntity
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
    }
}
