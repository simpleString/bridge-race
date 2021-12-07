
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI {
    public class MenuUI : MonoBehaviour {

        public GameObject SettingsMenu;
        public GameObject MainMenu;
        public GameObject ShopMenu;

        void Awake() {
            // Init store!!!
            Store.Store.Init();
            Time.timeScale = 1f;
        }

        void Start() {
            // #if !UNITY_EDITOR
            if (Store.Store.IsSoundOn) FindObjectOfType<AudioManager>().Play("Theme");
            // #endif
        }
        public void OnPlayButtonClicked() {
            SceneManager.LoadSceneAsync(Store.Store.CurrentLevel);
        }

        public void OnSettingsButtonClicked() {
            SettingsMenu.SetActive(true);
            MainMenu.SetActive(false);
        }

        public void OnCloseSettingsButtonClicked() {
            SettingsMenu.SetActive(false);
            MainMenu.SetActive(true);
        }

        public void OnShopButtonClicked() {
            MainMenu.SetActive(false);
            ShopMenu.SetActive(true);
        }

    }
}

