using ORM.Data;
using System;

namespace ORM.Tests
{
    public class Errors : DbEntity
    {
        public int ErrorID { get; set; }
        public string ActivityID { get; set; }
        public int? Number { get; set; }
        public string Source { get; set; }
        public string LocalSource { get; set; }
        public DateTime? Timestamp { get; set; }
        public int? Severity { get; set; }
        public int? PartyID { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string SubSystem { get; set; }
        public string ApplicationCD { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }
}