using System.Configuration;

namespace ORM.Configuration
{
    public class ORMConfiguration : ConfigurationSection
    {

        [ConfigurationProperty("connectionString", IsRequired = true)]
        public string ConnectionString
        {
            get
            {
                return (string)base["connectionString"];
            }
            set
            {
                base["connectionString"] = value;
            }
        }
    }
}
