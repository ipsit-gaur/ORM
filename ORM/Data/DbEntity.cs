namespace ORM.Data
{
    /// <summary>
    /// Baseclass for each Entity class representing a table schema of a DB
    /// </summary>
    public abstract class DbEntity
    {
        private DbEntityState _state = DbEntityState.Unchanged;
        private DbEntity _value;

        public DbEntity()
        {
            
        }
    }

    public enum DbEntityState
    {
        Unchanged,
        Modified,
        Added,
        Deleted
    }
}
