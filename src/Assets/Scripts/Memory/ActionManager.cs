using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Memory
{
    public class ActionManager : MonoBehaviour
    {
        public void GoTo(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
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