using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioCollection))]
public class AudioManagerScriptEditor : Editor {

    private AudioCollection customCollection;

    private SerializedProperty sPAudioCollections;

    private void OnEnable() {
        customCollection = (AudioCollection)target;
        sPAudioCollections = serializedObject.FindProperty("audioCollection");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.PropertyField(sPAudioCollections, true);
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Add", GUILayout.Width(100))) { 
            customCollection.audioCollection.Add(new AudioCollection.Audio());
        }
    }
}
