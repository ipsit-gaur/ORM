using System;

namespace ORM.Data
{
    /// <summary>
    /// Baseclass for each Entity class representing a table schema of a DB
    /// </summary>
    public abstract class DbEntity
    {
        public DbEntityState State = DbEntityState.Unchanged;

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
