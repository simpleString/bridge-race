
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI {
    public class MenuUI : MonoBehaviour {

        public GameObject SettingsMenu;
        public GameObject MainMenu;

        public Image VolumeImageObject;
        public Image VibrateImageObject;


        void Awake() {
            // VolumeImageObject.GetComponent<ImageConversion>
        }

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

        public void OnVolumeButtonClicked() {
            // VolumeSprite.GetComponent<Image
        }

        public void OnVibrateButtonClicked() {

        }
    }
}

