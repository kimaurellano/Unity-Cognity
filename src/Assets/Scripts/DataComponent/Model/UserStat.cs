using SQLite4Unity3d;

namespace Assets.Scripts.DataComponent.Model {
    public class UserStat {

        public enum GameCategory {
            Flexibility,
            Memory,
            Language,
            ProblemSolving,
            Speed
        }

        [PrimaryKey, AutoIncrement] public int Id { get; set; }

        public string Username { get; set; }

        public float Score { get; set; }

        public GameCategory Category { get; set; }
    }
}
