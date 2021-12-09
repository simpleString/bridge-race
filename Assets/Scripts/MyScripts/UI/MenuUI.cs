
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

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
            StartCoroutine(StartCameraAnimation());
        }

        IEnumerator StartCameraAnimation() {
            float animationTime = 2f;
            Transform cameraTransform = Camera.allCameras[0].transform;
            Vector3 startPosition = cameraTransform.position;
            Vector3 endPosition = new Vector3(0, 15, -16);

            for (float i = 0; i < animationTime; i += Time.deltaTime) {
                cameraTransform.DOLookAt(Vector3.up, i);
                // cameraTransform.LookAt(Vector3.Lerp(startPosition, Vector3.up, i / animationTime));
                cameraTransform.position = Vector3.Lerp(startPosition, endPosition, i / animationTime);
                yield return null;
            }
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

