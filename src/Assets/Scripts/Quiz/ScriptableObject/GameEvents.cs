using Assets.Scripts.Quiz.Mono;
using UnityEngine;

namespace Assets.Scripts.Quiz.ScriptableObject {
    [CreateAssetMenu(fileName = "GameEvents", menuName = "Quiz/new GameEvents")]
    public class GameEvents : UnityEngine.ScriptableObject {
        public delegate void DisplayResolutionScreenCallback(UIManager.ResolutionScreenType type, int score);

        public delegate void ScoreUpdatedCallback();

        public delegate void UpdateQuestionAnswerCallback(AnswerData pickedAnswer);

        public delegate void UpdateQuestionUICallback(Question question);

        [HideInInspector] public int CurrentFinalScore = 0;

        public DisplayResolutionScreenCallback DisplayResolutionScreen = null;

        public ScoreUpdatedCallback ScoreUpdated = null;

        [HideInInspector] public int StartupHighscore = 0;

        public UpdateQuestionAnswerCallback UpdateQuestionAnswer = null;

        public UpdateQuestionUICallback UpdateQuestionUI = null;
    }
}
