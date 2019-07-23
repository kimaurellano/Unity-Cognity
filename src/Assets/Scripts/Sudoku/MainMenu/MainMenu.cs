using UnityEngine;

public class MainMenu : MonoBehaviour {
    [SerializeField] private SelectLevel selectLevel = null;

    public void Display() {
        Debug.Log("Display Main Menu");
        gameObject.SetActive(true);
    }

    public void Hide() {
        Debug.Log("Hide Main Menu");
        gameObject.SetActive(false);
    }

    public void ButtonSelectLevel() {
        Debug.Log("Pressed Select Level button");
        selectLevel.Display();
        this.Hide();
    }
}
