using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    CharacterController charController;

    float horizontal;
    float vertical;

    public float speed = 6f;

    void Awake() {
        charController = GetComponent<CharacterController>();
    }

    void Update() {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate() {
        charController.SimpleMove(new Vector3(horizontal, 0, vertical) * speed);
    }
}
