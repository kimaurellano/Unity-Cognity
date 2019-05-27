using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Database.Model;
using SQLite4Unity3d;
using UnityEngine;

namespace Assets.Scripts.Database.Component {
    public class DataAccess {
        private static SQLiteConnection _connection;

        private const string DATABASE_NAME = "UserScore.db";

        public DataAccess() {
#if UNITY_EDITOR
            // Set database path to StreamingAssets folder
            var filepath = $@"Assets/StreamingAssets/{DATABASE_NAME}";
#else
            // Set database path to persistentDataPath within the android device
            var filepath = string.Format("{0}/{1}", Application.persistentDataPath, _databaseName);
#endif
#if UNITY_ANDROID
            // Check for database file in the persistentDataPath within the android device
            if (!File.Exists(filepath)) {
                // Open StreamingAssets directory and load the db
                var loadDb =
                    new WWW("jar:file://" +
                            Application.dataPath +
                            "!/assets/" +
                            DATABASE_NAME); // this is the path to your StreamingAssets in android
                while (!loadDb.isDone) {
                }

                File.WriteAllBytes(filepath, loadDb.bytes);
            }
#endif
            // Set connection to database
            _connection = new SQLiteConnection(filepath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        }

        public IEnumerable<UserScore> SelectAll() {
            return _connection.Table<UserScore>().OrderByDescending(i => i.Score);
        }
        
        public IEnumerable<UserScore> SelectUser(string username) {
            var userDetails = _connection.Table<UserScore>().Where(i => i.Username.Equals(username));

            return userDetails;
        }

        public void Insert(UserScore score) {
            if (!Exist(score.Username)) {
                // Add
                _connection.Insert(score);
            } else {
                // Update
                var curUserScore = GetScore(score.Username);
                if (curUserScore < score.Score) {
                    _connection.Update(score);
                }
            }
        }

        private static bool Exist(string username) {
            return _connection.Table<UserScore>().Where(i => i.Username.Equals(username)) != null;
        }

        private static float GetScore(string username) {
            var score = 0f;
            foreach (var userScore in _connection.Table<UserScore>().Where(i => i.Username.Equals(username))) {
                score = userScore.Score;
            }

            return score;
        }
    }
}
