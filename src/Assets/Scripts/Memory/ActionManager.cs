using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Memory
{
    public class ActionManager : MonoBehaviour
    {
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
    }
}