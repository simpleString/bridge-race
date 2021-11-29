using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI {
    public class VibrateButtonScript : MonoBehaviour {

        public Sprite ActiveImage;
        public Sprite DisableImage;

        public Image image;

        void Start() {
            UpdateImage();
        }

        void UpdateImage() {
            if (Store.Store.IsVibrationOn) {
                image.sprite = ActiveImage;
            } else {
                image.sprite = DisableImage;
            }
        }

        public void OnButtonClick() {
            Store.Store.IsVibrationOn = !Store.Store.IsVibrationOn;
            UpdateImage();
        }
    }
}