using System.Threading.Tasks;
using Assets.Scripts.GlobalScripts.Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Assets.Scripts.GlobalScripts.Player;

public class GameFinish : MonoBehaviour {
    [SerializeField] private Text resultText = null;
    [SerializeField] private Text timeValueText = null;
    [SerializeField] private Text movesValueText = null;

    private BaseScoreHandler _baseScoreHandler;

    public void Display(bool won, int elapsedSeconds, int moves) {
        Debug.Log("Display Game Finish message");

        if (won) resultText.text = "Won!";
        else resultText.text = "Lose!";

        timeValueText.text = $"{elapsedSeconds / 60:D2}:{elapsedSeconds % 60:D2}";
        movesValueText.text = $"{moves:D3}";

        _baseScoreHandler = new BaseScoreHandler();
        _baseScoreHandler.AddScore(float.Parse(movesValueText.text), Type.GameType.ProblemSolving);

        gameObject.SetActive(true);
    }

    public void Hide() {
        Debug.Log("Hide Game Finish message");
        gameObject.SetActive(false);
    }

    public void ButtonPlayAgain() {
        Debug.Log("Pressed Play Again button");
        LoadGameScene();
    }

    public void ButtonBackToMenu() {
        Debug.Log("Pressed Back To Menu button");
        LoadMainMenuScene();
    }

    private async void LoadGameScene() {
        Debug.Log("Load game scene");

        var asyncLoad = SceneManager.LoadSceneAsync("GameSudoku");
        while (!asyncLoad.isDone) await Task.Delay(15);
    }

    private async void LoadMainMenuScene() {
        Debug.Log("Load Main Menu scene");

        var asyncLoad = SceneManager.LoadSceneAsync("MenuSudoku");
        while (!asyncLoad.isDone) await Task.Delay(15);
    }
}
