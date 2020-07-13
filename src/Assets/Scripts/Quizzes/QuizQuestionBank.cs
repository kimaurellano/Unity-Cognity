using System;
using Assets.Scripts.GlobalScripts.Game;

namespace Assets.Scripts.Quizzes {
    [Serializable]
    public class QuestionBankQuiz {

        public string Question;

        public Difficulty.DifficultyLevel Difficulty;

        public Answer[] Answers;

        [Serializable]
        public struct Answer {
            public string AnswerText;

            public bool IsCorrect;
        }
    }
}
