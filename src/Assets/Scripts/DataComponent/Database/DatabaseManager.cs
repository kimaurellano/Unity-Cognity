using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Assets.Scripts.DataComponent.Model;
using SQLite4Unity3d;
using UnityEngine;

namespace Assets.Scripts.DataComponent.Database {
    public class DatabaseManager {

        private static SQLiteConnection _connection;

        private const string DATABASE = "UserData.db";

        public DatabaseManager() {
#if UNITY_EDITOR
            // Set database path to StreamingAssets folder
            var filepath = $@"Assets/StreamingAssets/{DATABASE}";
#else
        // Set database path to persistentDataPath within the android device
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DATABASE);
#endif
#if UNITY_ANDROID
            // Check for database file in the persistentDataPath within the android device
            if (!File.Exists(filepath)) {
                // Open StreamingAssets directory and load the db
                var loadDb =
                    new WWW("jar:file://" +
                            Application.dataPath +
                            "!/assets/" +
                            DATABASE); // this is the path to your StreamingAssets in android
                while (!loadDb.isDone) {
                }

                File.WriteAllBytes(filepath, loadDb.bytes);
            }
#endif
            // Set connection to database
            _connection = new SQLiteConnection(filepath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        }

        public void DeletePersistentData() {
            var filepath = $"{Application.persistentDataPath}/{DATABASE}";
            File.Delete(filepath);
        }

        public void CreateNewUser(User user) {
            if (GetUser(user.Username)?.Username != null) {
                Debug.Log("<color=yellow>User exists already</color>");
                return;
            }

            _connection.Insert(user);
            Debug.Log("<color=green>User created</color>");

            _connection.Query<UserStat>($"INSERT INTO UserStat VALUES(NULL, '{user.Username}', 0.0, {(int)UserStat.GameCategory.Flexibility})");
            _connection.Query<UserStat>($"INSERT INTO UserStat VALUES(NULL, '{user.Username}', 0.0, {(int)UserStat.GameCategory.Language})");
            _connection.Query<UserStat>($"INSERT INTO UserStat VALUES(NULL, '{user.Username}', 0.0, {(int)UserStat.GameCategory.Memory})");
            _connection.Query<UserStat>($"INSERT INTO UserStat VALUES(NULL, '{user.Username}', 0.0, {(int)UserStat.GameCategory.ProblemSolving})");
            _connection.Query<UserStat>($"INSERT INTO UserStat VALUES(NULL, '{user.Username}', 0.0, {(int)UserStat.GameCategory.Speed})");
            Debug.Log("<color=green>User score stat created</color>");
        }

        public void UpdateUser(string username, UserPrefs updatedUser) {
            UserPrefs existingUser = _connection.Query<UserPrefs>($"SELECT * FROM UserPrefs WHERE Username='{username}'").FirstOrDefault();
            if (existingUser == null) {
                Debug.Log("<color=red>User non-existent</color>");
                return;
            }

            existingUser = updatedUser;

            _connection.RunInTransaction(() => { _connection.Update(existingUser); });
            Debug.Log("<color=green>User updated</color>");
        }

        public void UpdateUserStat(string username, UserStat updatedUserStat, UserStat.GameCategory category) {
            UserStat existingUser = 
                _connection.Query<UserStat>($"SELECT * FROM UserStat WHERE Username='{username}' AND Category={(int)category}").FirstOrDefault();
            if (existingUser == null) {
                Debug.Log("<color=red>User non-existent</color>");
                return;
            }

            existingUser = updatedUserStat;

            _connection.RunInTransaction(() => { _connection.Update(existingUser); });
            Debug.Log($"<color=green>User score updated at category:{(int)category}</color>");

            // Record the score per game taken. Accumulated to compute for overall 
            // session score which will serve as score history
            SaveSessionScore(username, existingUser.Score, category);
        }

        public void DeleteAllData() {
            _connection.DeleteAll<UserStat>();
            _connection.DeleteAll<UserScoreHistory>();
            _connection.DeleteAll<UserPrefs>();

            _connection.Close();
        }

        public void DeleteUser(string username) {
            if (GetUser(username).Username == null) {
                Debug.Log("<color=red>UserPrefs non-existent</color>");
                return;
            }
            _connection.Query<UserPrefs>($"DELETE FROM UserPrefs WHERE Username='{username}'");

            Debug.Log("<color=green>UserPrefs deleted</color>");
        }

        public UserPrefs GetUser(string username) {
            return _connection.Query<UserPrefs>($"SELECT * FROM UserPrefs WHERE Username='{username}'").FirstOrDefault();
        }
        
        public IEnumerable<UserPrefs> GetUsers() {
            return _connection.Query<UserPrefs>("SELECT * FROM UserPrefs");
        }

        public IEnumerable<UserStat> GetUserStats() {
            return _connection.Query<UserStat>("SELECT * FROM UserStat");
        }

        public UserStat GetUserStat(string username, UserStat.GameCategory category) {
            return _connection.Query<UserStat>($"SELECT * FROM UserStat WHERE Username='{username}' AND Category={(int)category}").FirstOrDefault();
        }

        public IEnumerable<UserScoreHistory> GetScoreHistory(string username) {
            return _connection.Query<UserScoreHistory>($"SELECT * FROM UserScoreHistory WHERE Username='{username}' ORDER BY Time ASC");
        }

        public IEnumerable<UserScoreHistory> GetScoreHistory(string username, UserStat.GameCategory category) {
            return _connection.Query<UserScoreHistory>($"SELECT * FROM UserScoreHistory WHERE Username='{username}' AND Category={(int)category} ORDER BY Time ASC");
        }

        private static void SaveSessionScore(string username, float score, UserStat.GameCategory category) {
            _connection
                .Query<UserScoreHistory>(
                    "INSERT INTO UserScoreHistory " +
                    $"VALUES(NULL, '{username}', {score}, '{DateTime.Now.ToString(CultureInfo.InvariantCulture)}', {(int)category})");

            Debug.Log("<color=green>Score logged</color>");
        }

        public void Close() {
            _connection.Close();
        }
    }
}