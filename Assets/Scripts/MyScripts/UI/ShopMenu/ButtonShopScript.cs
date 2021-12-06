using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UI {
    public class ButtonShopScript : MonoBehaviour {
        public Color color;
        Color initButtonColor;
        GameManager.MyColor colorName;

        public Image innerColor;
        public Image myColor;
        void Awake() {

            myColor = GetComponent<Image>();
            // material = GetComponent<Renderer>().material;
        }

        public void Init(Color _color, GameManager.MyColor name) {
            innerColor = GetComponentsInChildren<Image>()[1];
            this.color = _color;
            this.colorName = name;
            this.initButtonColor = myColor.color;

        }

        void Update() {
            // myColor.color = myColor;
            if (innerColor != null)
                innerColor.color = color;
            if (Store.Store.PlayerColor == colorName) {
                myColor.color = new Color(.95f, .16f, .16f);
            } else {
                myColor.color = initButtonColor;
            }
        }

        public void OnButtonClick() {
            Store.Store.PlayerColor = colorName;
        }
    }
}