using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SideScript : MonoBehaviour {

    private Collider collider;

    void Awake() {
        collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Free")) {
            foreach (var item in GameObject.FindObjectsOfType<BrickSpawner>()) {
                Debug.Log("Randomize bricks on platforms");
                item.UpdateOneRandomBrick();
            }
        }
        Destroy(collider.gameObject);
    }
}
