using System;
using UnityEngine;

public class Bonus : MonoBehaviour {

    public GameManager.BonusType type;

    float RotateSpeed = 50f;

    void Update() {
        transform.Rotate(Vector3.down, Time.deltaTime * RotateSpeed);
    }

    public void Destory() {
        Destroy(this.gameObject);
    }
}