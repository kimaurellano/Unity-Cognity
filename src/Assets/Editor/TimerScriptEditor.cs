using Assets.Scripts.GlobalScripts.Managers;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.Game {
    [CustomEditor(typeof(TimerManager))]
    class TimerScriptEditor : Editor {

        private TimerManager _myTimerManager;

        public override void OnInspectorGUI() {
            _myTimerManager = (TimerManager) target;

            if (_myTimerManager.IsPreGameTimer) {
                if (_myTimerManager.AttachedTextObject == null) {
                    _myTimerManager.IsPreGameTimer = false;
                } else {
                    EditorGUI.BeginDisabledGroup(true);
                    _myTimerManager.TimerText =
                        (TextMeshProUGUI)EditorGUILayout.ObjectField(_myTimerManager.TimerText, typeof(TextMeshProUGUI), true);
                    _myTimerManager.TimerText = _myTimerManager.AttachedTextObject;
                    EditorGUI.EndDisabledGroup();
                }
            } else {
                _myTimerManager.TimerText =
                    (TextMeshProUGUI)EditorGUILayout.ObjectField(_myTimerManager.TimerText, typeof(TextMeshProUGUI), true);
            }

            EditorGUI.BeginDisabledGroup(_myTimerManager.IsPreGameTimer);
            _myTimerManager.Minutes = EditorGUILayout.IntField("Minutes", _myTimerManager.Minutes);
            EditorGUI.EndDisabledGroup();

            _myTimerManager.Seconds = EditorGUILayout.FloatField("Seconds", _myTimerManager.Seconds);
            EditorGUILayout
                .HelpBox(
                    "This script should be attached to a gameobject " +
                    "with TextMeshProUGUI component if enabled", MessageType.Info);
            _myTimerManager.IsPreGameTimer = EditorGUILayout.Toggle("Pre-Game Timer", _myTimerManager.IsPreGameTimer);

            _myTimerManager.TimerAnimation =
                (Animation)EditorGUILayout.ObjectField(_myTimerManager.TimerAnimation, typeof(Animation), true);
        }
    }
}