using Assets.Scripts.Database.Enum;

namespace Assets.Scripts.Database.Interface {
    public interface IScoreHandler {
        void SaveScore(float score, Game.GameType gameType);
    }
}
