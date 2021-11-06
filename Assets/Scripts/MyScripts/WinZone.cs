using UnityEngine;


public class WinZone : MonoBehaviour {
    void OnTriggerEnter(Collider c) {
        if (c.tag == "Player") {
            Debug.Log("player win game");
        }
    }
}