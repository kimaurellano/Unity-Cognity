using SQLite4Unity3d;

namespace Assets.Scripts.DataComponent.Model {
    public class UserPrefs {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Username { get; set; }

        public bool FirstRun { get; set; }

        public bool IsLogged { get; set; }

        public string PageLoaded { get; set; }
    }
}
