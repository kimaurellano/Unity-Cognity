using Assets.Scripts.GlobalScripts.UIComponents;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.UITask {
    /// <summary>
    ///     Collection of UI Elements to be modified on runtime.
    /// </summary>
    public class UIManager : MonoBehaviour {
        [SerializeField] private StatsCollection[] _statsCollections;
        [SerializeField] private PanelCollection[] _panelCollection;
        [SerializeField] private ButtonCollection[] _buttonCollection;
        [SerializeField] private TextCollection[] _textCollection;

        public PanelCollection[] PanelCollection { get => _panelCollection; set => _panelCollection = value; }
        public ButtonCollection[] ButtonCollection { get => _buttonCollection; set => _buttonCollection = value; }
        public TextCollection[] TextCollection { get => _textCollection; set => _textCollection = value; }
        public StatsCollection[] StatsCollections { get => _statsCollections; set => _statsCollections = value; }
    }
}
