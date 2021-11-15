using UnityEngine;


public class WinZone : MonoBehaviour {
    void OnTriggerEnter(Collider c) {
        if (c.tag == "Player") {
            GameManager.Instance.GameWin(c.gameObject);
        }
    }
}