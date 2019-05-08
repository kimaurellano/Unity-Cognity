using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Quiz.Mono
{
    public class LoadQuestion : MonoBehaviour
    {

        #region Variables

        private Question[] _questions = null;
        public Question[] Questions { get { return _questions; } }

		#endregion

        void LoadQuestions()
        {
			Object[] objs = Resources.LoadAll("MathQuizQuestions", typeof(Question));
			_questions = new Question[20];
			for (int i = 0; i < 20; i++)
            {
                _questions[i] = (Question)objs[i];
            }

		}
	}
}