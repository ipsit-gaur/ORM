using System.ComponentModel;

namespace ORM.Data
{
    /// <summary>
    /// Baseclass for each Entity class representing a table schema of a DB
    /// </summary>
    public abstract class DbEntity : INotifyPropertyChanged
    {
        private DbEntityState _state = DbEntityState.Unchanged;

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public enum DbEntityState
    {
        Unchanged,
        Modified,
        Added,
        Deleted
    }
}
