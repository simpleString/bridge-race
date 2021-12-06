using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UI {
    public class ColorShopScript : MonoBehaviour {
        public GameObject shopCell;

        public int countOfCells;

        void Awake() {
            foreach (GameManager.MyColor item in Enum.GetValues(typeof(GameManager.MyColor))) {
                if (item == GameManager.MyColor.black) continue;
                var newShopCell = Instantiate(shopCell, transform);
                newShopCell.GetComponent<ButtonShopScript>().Init(GameManager.GetUnityColorByMyColor(item), item);
            }
        }

    }
}
