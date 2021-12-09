using UnityEngine.UI;
using UnityEngine;

namespace UI {
    public class LevelMenu : MonoBehaviour {
        public Button button1;
        public Button button2;
        public Button button3;


        void Awake() {
            button1.onClick.AddListener(SetLevel1);
            button2.onClick.AddListener(SetLevel2);
            button3.onClick.AddListener(SetLevel3);
        }

        void SetLevel1() {
            FindObjectOfType<LevelManager>().SetLevel("Level_1");
        }

        void SetLevel2() {
            FindObjectOfType<LevelManager>().SetLevel("Level_2");
        }

        void SetLevel3() {
            FindObjectOfType<LevelManager>().SetLevel("Level_3");
        }
    }
}