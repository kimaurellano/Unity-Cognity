using SQLite4Unity3d;
using UnityEngine;

namespace Assets.Scripts.DataComponent.Model {
    public class UserStat : MonoBehaviour {
        [PrimaryKey, AutoIncrement] public int Id { get; set; }

        public string Username { get; set; }

        public float Score { get; set; }

        public enum GameCategory {
            Flexibility,

            Language,

            Memory,

            ProblemSolving
        }

        public GameCategory Category { get; set; }
    }
}
