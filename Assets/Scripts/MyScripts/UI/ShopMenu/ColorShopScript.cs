using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UI {
    public class ColorShopScript : MonoBehaviour {
        public GameObject shopCell;

        public int countOfCells;

        void Awake() {
            var i = 0;
            while (i < countOfCells) {
                Instantiate(shopCell, transform);
                i++;
            }
        }

    }
}

