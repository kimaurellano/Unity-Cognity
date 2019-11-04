using SQLite4Unity3d;
using UnityEngine;

namespace Assets.Scripts.DataComponent.Model {
    public class UserScoreHistory : MonoBehaviour {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Username { get; set; }

        public float SessionScore { get; set; }

        public string Time { get; set; }
    }
}
