
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
    public class MenuUI : MonoBehaviour {

        public void OnPlayButtonClicked() {
            SceneManager.LoadScene("Game");
        }

        public void OnExitButtonClicked() {
            Application.Quit();
        }
    }
}

