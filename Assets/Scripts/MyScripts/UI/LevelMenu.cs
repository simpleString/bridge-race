using UnityEngine.UI;
using UnityEngine;

namespace UI {
    public class LevelMenu : MonoBehaviour {
        public Button button1;
        public Button button2;


        void Awake() {
            button1.onClick.AddListener(SetLevel1);
            button2.onClick.AddListener(SetLevel2);
        }

        void SetLevel1() {
            FindObjectOfType<LevelManager>().SetLevel("Level_1");
        }

        void SetLevel2() {
            FindObjectOfType<LevelManager>().SetLevel("Level_2");
        }
    }
}