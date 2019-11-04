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

        public User GetUser(string username) {
            return _connection.Query<User>($"SELECT * FROM User WHERE Username='{username}'").FirstOrDefault();
        }

        public IEnumerable<User> GetUsers() {
            return _connection.Query<User>("SELECT * FROM User");
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
            Debug.Log("<color=green>User score stat created</color>");
        }

        public void UpdateUser(string username, User updatedUser) {
            User existingUser = _connection.Query<User>($"SELECT * FROM User WHERE Username='{username}'").FirstOrDefault();
            if (existingUser == null) {
                Debug.Log("<color=red>User non-existent</color>");
                return;
            }

            existingUser.Username = updatedUser.Username;
            existingUser.FirstRun = updatedUser.FirstRun;
            existingUser.IsLogged = updatedUser.IsLogged;
            _connection.RunInTransaction(() => { _connection.Update(existingUser); });
            Debug.Log("<color=green>User updated</color>");
        }

        public void DeleteUser(string username) {
            if (GetUser(username).Username == null) {
                Debug.Log("<color=red>User non-existent</color>");
                return;
            }
            _connection.Query<User>($"DELETE FROM User WHERE Username='{username}'");

            Debug.Log("<color=green>User deleted</color>");
        }

        public void SaveScore(string username, float score, UserStat.GameCategory category) {
            UserStat userStat = _connection.Query<UserStat>($"SELECT * FROM UserStat WHERE Username='{username}' AND Category={category}").FirstOrDefault();
            if (userStat != null) {
                // Normalize score
                userStat.Score = (score + userStat.Score) / 2;
                _connection.RunInTransaction(() => { _connection.Update(userStat); });
                Debug.Log($"<color=green>User({username}) score updated in category:{category}</color>");

                // Record the score per game taken. Accumulated to compute for overall 
                // session score which will serve as score history
                SaveSessionScore(username, userStat.Score);
            }
        }

        public IEnumerable<UserScoreHistory> GetScoreHistory(string username) {
            return _connection.Query<UserScoreHistory>($"SELECT * FROM UserScoreHistory WHERE Username='{username}' ORDER BY DATE ASC");
        }

        public void Close() {
            _connection.Close();
        }

        private static void SaveSessionScore(string username, float score) {
            _connection
                .Query<UserScoreHistory>(
                    "INSERT INTO UserScoreHistory " +
                    $"VALUES(NULL, '{username}', {score}, '{DateTime.Now.ToString(CultureInfo.InvariantCulture)}')");

            Debug.Log("<color=green>Score logged</color>");
        }
    }
}