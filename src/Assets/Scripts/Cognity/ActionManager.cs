using System;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using Assets.Scripts.GlobalScripts;

namespace Assets.Scripts.Cognity
{
    /// <summary>
    /// Collection of Button's actions
    /// </summary>    
    public class ActionManager : MonoBehaviour
    {

        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                switch (SceneManager.GetActiveScene().buildIndex)
                {
                    case 1:
                        Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name == "pause panel")
                            .Panel
                            .gameObject
                            .SetActive(true);
                        break;
                    case 0:
                        Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name == "quit panel")
                            .Panel
                            .gameObject
                            .SetActive(true);
                        break;
                }
            }
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void Pause()
        {
            Array.Find(FindObjectOfType<UIManager>().PanelCollection, i => i.Name == "pause panel")
                .Panel
                .gameObject
                .SetActive(true);
        }

        public void LoadScene(string sceneName)
        {
            if (sceneName == "BaseMenu")
            {
                Destroy(GameObject.Find("AudioManager").gameObject);
            }

            SceneManager.LoadScene(sceneName);
        }

        public void Hide(Transform panel)
        {
            panel.gameObject.SetActive(false);
        }

        public void Show(Transform panel)
        {
            panel.gameObject.SetActive(true);
        }

        public void SaveUserPref(TMP_InputField input)
        {
            string userInfo = input.text;
            if (input.text == string.Empty)
            {
                System.Random rand = new System.Random();
                userInfo = "user" + rand.NextDouble();
            }

            // Clear first
            PlayerPrefs.DeleteAll();

            // Cache user name
            PlayerPrefs.SetString("user_info", userInfo);
        }

        public void InvokeAnimation(Animator animator)
        {
            animator.SetTrigger("show");
        }

        public void MuteBackground()
        {
            bool mute = Array.Find(FindObjectOfType<AudioManager>().AudioCollection, s => s.Name == "background").AudioSource.mute;
            Array.Find(FindObjectOfType<AudioManager>().AudioCollection, s => s.Name == "background").AudioSource.mute = !mute;

            if (mute)
            {
                Transform button = Array.Find(FindObjectOfType<UIManager>().ButtonCollection, i => i.Name == "music").Button;
                button.GetChild(0).gameObject.SetActive(false);
                button.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                Transform button = Array.Find(FindObjectOfType<UIManager>().ButtonCollection, i => i.Name == "music").Button;
                button.GetChild(0).gameObject.SetActive(true);
                button.GetChild(1).gameObject.SetActive(false);
            }
        }
    }
}
