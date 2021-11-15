
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
    public class MenuUI : MonoBehaviour {

        public void OnPlayButtonClicked() {
            SceneManager.LoadScene("SampleScene");
        }

        public void OnExitButtonClicked() {
            Application.Quit();
        }
    }
}

