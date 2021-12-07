using UnityEngine;


public class WinZone : MonoBehaviour {

    public Animator doorAnimator;

    void OnTriggerEnter(Collider c) {
        if (c.tag == "Player") {
            doorAnimator.SetBool("CloseDoor", true);
            GameManager.Instance.GameWin(c.gameObject);
        }
    }
}