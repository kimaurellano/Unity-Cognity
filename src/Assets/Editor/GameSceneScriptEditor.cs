using UnityEditor;
using static AudioCollection.Audio;

[CustomEditor(typeof(GameSceneManager))]
public class SceneScriptEditor : Editor {

    private int _selected;

    public override void OnInspectorGUI() {
        GameSceneManager sceneManager = (GameSceneManager)target;

        string[] sceneTypes = { "Main Menu", "In Game", "Interactable" };

        _selected = EditorGUILayout.Popup("Scene type", _selected, sceneTypes);

        switch (_selected + 1) {
        case 1:
            sceneManager.CurrentSceneType = UseType.MainMenu;
            break;
        case 2:
            sceneManager.CurrentSceneType = UseType.InGame;
            break;
        }
    }
}
