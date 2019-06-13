using System;
using Assets.Scripts.PicturePuzzle;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI _inputField;
    [SerializeField] private TextMeshProUGUI _placeHolder;
    [SerializeField] private GameObject _answerState;
    [SerializeField] private GameObject _puzzlePictureContainer;
    [SerializeField] private PicturePuzzleCollection[] _picturePuzzleCollections;

    private int currentNumber = 1;

    private void Start() {
        Instantiate(Array.Find(_picturePuzzleCollections, i => i.puzzleId == currentNumber).Image, _puzzlePictureContainer.transform);
    }

    public void CheckAnswer() {
        string answer = Array.Find(_picturePuzzleCollections, i => i.puzzleId == currentNumber).Answer;
        if (_inputField.text.Contains(answer)) {
            _answerState.GetComponent<TextMeshProUGUI>().SetText("CORRECT!");
            _answerState.GetComponent<Animator>().SetTrigger("correct");

            // Clear picture puzzle container before next puzzle
            Destroy(_puzzlePictureContainer.transform.GetChild(0).gameObject);

            NextPuzzle();
        }
        else {
            _answerState.GetComponent<TextMeshProUGUI>().SetText("WRONG");
            _answerState.GetComponent<Animator>().SetTrigger("wrong");
        }
    }

    public void NextPuzzle() {
        currentNumber++;

        // Prevent null exception
        if (currentNumber > _picturePuzzleCollections.Length) {
            return;
        }

        Instantiate(Array.Find(_picturePuzzleCollections, i => i.puzzleId == currentNumber).Image, _puzzlePictureContainer.transform);
    }

    public void ClearField() {
        _placeHolder.text = string.Empty;
    }
}
