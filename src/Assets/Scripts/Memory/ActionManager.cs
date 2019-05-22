using System;
using Assets.Scripts.GlobalScripts;
using Assets.Scripts.GlobalScripts.UITask;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Memory
{
    public class ActionManager : MonoBehaviour
    {
        private void Start() 
        {
            // Not first time use
            if (PlayerPrefs.GetString("user_info") != string.Empty) 
            {
                Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name.Equals("panel home"))
                    .Panel
                    .transform
                    .gameObject
                    .SetActive(true);

                Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name.Equals("panel userinfo"))
                    .Panel
                    .transform
                    .gameObject
                    .SetActive(false);
            }
        }

        public void GoTo(string sceneName)
        {
            if (sceneName == "BaseMenu") 
            {
                Destroy(GameObject.Find("AudioManager").gameObject);
            }

            SceneManager.LoadScene(sceneName);
        }

        public void GoToBaseMenu() 
        {
            SceneManager.LoadScene("BaseMenu");
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void Show(Transform transform)
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide(Transform transform)
        {
            transform.gameObject.SetActive(false);
        }

        public void SaveUserPref(TMP_InputField input) 
        {
            string userInfo = input.text;
            if (input.text == string.Empty) 
            {
                System.Random rand = new System.Random();
                userInfo = "user" + rand.NextDouble();
            }

            // Cache user name
            PlayerPrefs.SetString("user_info", userInfo);
        }
    }
}