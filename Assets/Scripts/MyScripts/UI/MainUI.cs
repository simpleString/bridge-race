
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
    public class MainUI : MonoBehaviour {

        private bool isMenuOpen = false;

        public GameObject pauseMenu;

        public GameObject winMenu;
        public GameObject loseMenu;

        public void OnSettingButtonClicked() {
            isMenuOpen = true;
            pauseMenu.SetActive(isMenuOpen);
            Time.timeScale = 0;
        }

        public void OnResumeButtonClicked() {
            isMenuOpen = false;
            pauseMenu.SetActive(isMenuOpen);
            Time.timeScale = 1;
        }

        public void OnMenuButtonClicked() {
            SceneManager.LoadScene("Menu");
        }

        public void OnWinTrigger() {
            winMenu.SetActive(true);
        }

        public void OnLoseTrigger() {
            if (!winMenu.activeSelf)
                loseMenu.SetActive(true);
        }

        public void OnPlayAgainButtonClicked() {
            SceneManager.LoadScene("Game");
        }
    }
}


