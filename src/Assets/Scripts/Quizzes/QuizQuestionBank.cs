using System;

namespace Assets.Scripts.Quizzes {
    [Serializable]
    public class QuestionBankQuiz {
        public  enum Difficulty {
            Easy,
            Medium,
            Hard
        }

        public string Question;

        public Difficulty QuestionDifficulty;

        public Answer[] Answers;

        [Serializable]
        public struct Answer {
            public string AnswerText;

            public bool IsCorrect;
        }
    }
}
