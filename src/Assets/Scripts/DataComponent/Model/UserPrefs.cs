using System.Linq;
using Assets.Scripts.DataComponent.Database;
using SQLite4Unity3d;

namespace Assets.Scripts.DataComponent.Model {
    public class UserPrefs {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Username { get; set; }

        public bool FirstRun { get; set; }

        public bool IsLogged { get; set; }

        public string PageLoaded { get; set; }

        public bool InSession { get; set; }

        public static void UpdateUserPrefs(string pageToLoad) {
            DatabaseManager databaseManager = new DatabaseManager();
            UserPrefs userPrefs = databaseManager.GetUsers().FirstOrDefault(i => i.IsLogged);
            if (userPrefs != null) {
                userPrefs.PageLoaded = pageToLoad;
            }

            databaseManager.UpdateUser(userPrefs?.Username, userPrefs);
            databaseManager.Close();
        }

        public static void UpdateUserPrefs(string pageToLoad, bool isLogged) {
            DatabaseManager databaseManager = new DatabaseManager();
            UserPrefs userPrefs = databaseManager.GetUsers().FirstOrDefault(i => i.IsLogged);
            if (userPrefs != null) {
                userPrefs.PageLoaded = pageToLoad;
                userPrefs.IsLogged = isLogged;
            }

            databaseManager.UpdateUser(userPrefs?.Username, userPrefs);
            databaseManager.Close();
        }

        /// <summary>
        /// Upon game log-in
        /// </summary>
        public static void UpdateUserPrefs(string username, string pageToLoad, bool isLogged) {
            DatabaseManager databaseManager = new DatabaseManager();
            UserPrefs userPrefs = databaseManager.GetUser(username);
            if (userPrefs != null) {
                userPrefs.PageLoaded = pageToLoad;
                userPrefs.IsLogged = isLogged;
            }

            databaseManager.UpdateUser(userPrefs?.Username, userPrefs);
            databaseManager.Close();
        }

        public static void UpdateUserPrefs(bool inSession) {
            DatabaseManager databaseManager = new DatabaseManager();
            UserPrefs userPrefs = databaseManager.GetUsers().FirstOrDefault(i => i.IsLogged);
            if (userPrefs != null) {
                userPrefs.InSession = inSession;
            }

            databaseManager.UpdateUser(userPrefs?.Username, userPrefs);
            databaseManager.Close();
        }

        public static bool SessionActive() {
            DatabaseManager databaseManager = new DatabaseManager();
            UserPrefs first = databaseManager.GetUsers().FirstOrDefault(i => i.IsLogged);
            return first != null && first.InSession;
        }
    }
}
