using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Database.Model;
using SQLite4Unity3d;
using UnityEngine;

namespace Assets.Scripts.Database
{
    public class DataAccess
    {

        private static SQLiteConnection _connection;
        private readonly string _databaseName = "UserScore.db";

        public DataAccess()
        {
#if UNITY_EDITOR
            // Set database path to StreamingAssets folder
            var filepath = $@"Assets/StreamingAssets/{_databaseName}";
#else
            // Set database path to persistentDataPath within the android device
            var filepath = string.Format("{0}/{1}", Application.persistentDataPath, _databaseName);
#endif
#if UNITY_ANDROID
            // Check for database file in the persistentDataPath within the android device
            if (!File.Exists(filepath)) {
                // Open StreamingAssets directory and load the db
                var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + _databaseName);  // this is the path to your StreamingAssets in android
                while (!loadDb.isDone) ;

                File.WriteAllBytes(filepath, loadDb.bytes);
            }
#endif
            // Set connection to database
            _connection = new SQLiteConnection(filepath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        }

        public IEnumerable<UserScore> SelectAll()
        {
            return _connection.Table<UserScore>().OrderByDescending(i => i.Score);
        }

        public void Insert(UserScore score)
        {
            _connection.Insert(score);
        }
    }
}