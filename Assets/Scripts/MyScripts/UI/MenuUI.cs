
using UnityEngine;
using UnityEngine.SceneManagement;


namespace UI {
    public class MenuUI : MonoBehaviour {

        public GameObject SettingsMenu;
        public GameObject MainMenu;

        public void OnPlayButtonClicked() {
            SceneManager.LoadScene("Game");
        }

        public void OnSettingsButtonClicked() {
            SettingsMenu.SetActive(true);
            MainMenu.SetActive(false);
        }

        public void OnCloseSettingsButtonClicked() {
            SettingsMenu.SetActive(false);
            MainMenu.SetActive(true);
        }
    }
}

