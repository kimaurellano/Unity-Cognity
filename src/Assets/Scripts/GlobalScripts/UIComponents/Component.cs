using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GlobalScripts.UIComponents {
    public class Component : MonoBehaviour {
        public string Name;

        public enum Type {
            Panel,

            Text,

            InputField,

            AnimatedObjectSingleState,

            AnimatedObjectMultipleState
        }

        public List<IComponent> UIComponents;
    }
}