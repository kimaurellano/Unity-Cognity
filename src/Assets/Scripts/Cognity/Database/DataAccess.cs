using System.IO;
using SQLite4Unity3d;
using UnityEngine;
using Assets.Scripts.Cognity.Database.Model;
using System.Collections.Generic;

namespace Assets.Scripts.Cognity.Database
{
    public class DataAccess
    {

        private static SQLiteConnection _connection;
        private readonly string _databaseName = "UserScore.db";

        public DataAccess()
        {
#if UNITY_EDITOR
            // Set database path to StreamingAssets folder
            var filepath = string.Format(@"Assets/StreamingAssets/{0}", _databaseName);
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
            // Check and update the username if it does exist
            foreach (var item in SelectAll())
            {
                if (item.Username == score.Username)
                {
                    _connection.Update(score);

                    return;
                }
            }
            _connection.Insert(score);
        }
    }
}