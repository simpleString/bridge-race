using UnityEngine;


namespace UI {
    [CreateAssetMenu(fileName = "ColorItem", menuName = "ScriptableObjects/ColorItemSrciptableObject")]



    public class ItemManagerScriptableObject : ScriptableObject {
        public string Name;
        public Color Color;

        void Awake() {

        }

    }
}