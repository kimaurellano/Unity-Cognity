using System;
using Assets.Scripts.GlobalScripts.UIComponents;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.Managers {
    /// <summary>
    ///     Collection of UI Elements to be modified on runtime.
    /// </summary>
    public class UIManager : MonoBehaviour {
        public enum UIType {
            Panel,
            Button,
            Text,
            InputField,
            AnimatedSingleState,
            AnimatedMultipleState
        }

        [Space]
        [SerializeField] private StatsCollection[] _statsCollections;
        [Space]
        [SerializeField] private PanelCollection[] _panelCollection;
        [Space]
        [SerializeField] private ButtonCollection[] _buttonCollection;
        [Space]
        [SerializeField] private TextCollection[] _textCollection;
        [Space]
        [SerializeField] private InputFieldCollection[] _inputFieldCollection;
        [Space]
        [SerializeField] private AnimatedObjects[] _animatedObjectsCollecton;

        public PanelCollection[] PanelCollection { get => _panelCollection; set => _panelCollection = value; }
        public ButtonCollection[] ButtonCollection { get => _buttonCollection; set => _buttonCollection = value; }
        public TextCollection[] TextCollection { get => _textCollection; set => _textCollection = value; }
        public StatsCollection[] StatsCollections { get => _statsCollections; set => _statsCollections = value; }
        public InputFieldCollection[] InputFieldCollections { get => _inputFieldCollection; set => _inputFieldCollection = value; }
        public AnimatedObjects[] AnimatedObjectsCollection { get => _animatedObjectsCollecton; set => _animatedObjectsCollecton = value; }

        public object GetUI(UIType type, string uiName) {
            switch (type) {
            case UIType.Text:
                return Array.Find(TextCollection, collection => collection.Name.Equals(uiName)).textMesh;
            case UIType.Button:
                return Array.Find(ButtonCollection, collection => collection.Name.Equals(uiName)).Button;
            case UIType.AnimatedSingleState:
                return Array.Find(AnimatedObjectsCollection, collection => collection.Name.Equals(uiName)).AnimatedObject.GetComponent<Animation>();
            case UIType.AnimatedMultipleState:
                return Array.Find(AnimatedObjectsCollection, collection => collection.Name.Equals(uiName)).AnimatedObject.GetComponent<Animator>();
            case UIType.Panel:
                return Array.Find(PanelCollection, collection => collection.Name.Equals(uiName)).Panel;
            case UIType.InputField:
                return Array.Find(InputFieldCollections, collection => collection.Name.Equals(uiName)).InputField;
            }
            return null;
        } 
    }
}
