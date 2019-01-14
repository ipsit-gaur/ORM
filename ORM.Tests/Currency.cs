using ORM.Attributes;
using ORM.Data;

namespace ORM.Tests
{
    public class Currency : DbEntity
    {
        [Key]
        public string CurrencyCD { get; set; }
        public string Description { get; set; }
        public string Symbol { get; set; }
        public decimal USDRate { get; set; }
    }
}
