using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI {
    public class VolumeButtonScript : MonoBehaviour {

        public Sprite ActiveImage;
        public Sprite DisableImage;

        public Image image;

        void Start() {
            UpdateImage();
        }

        void UpdateImage() {
            image.sprite = Store.Store.IsSoundOn ? ActiveImage : DisableImage;
        }

        public void OnButtonClick() {
            Store.Store.IsSoundOn = !Store.Store.IsSoundOn;
            if (!Store.Store.IsSoundOn) {
                FindObjectOfType<AudioManager>().StopAll();
            } else {
                FindObjectOfType<AudioManager>().Play("Theme");
            }
            UpdateImage();
        }
    }
}