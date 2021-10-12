using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof (Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Vector3 _movement;

    public Rigidbody playerRigidbody;

    Player _player;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        _player = GetComponent<Player>();
    }

    public void Move(Vector3 movement)
    {
        // _velocity = Vector3.forward * velocity;
        // _rotation = Vector3.right * rotation;
       _movement = movement;

    }

    void FixedUpdate()
    {
        if (_player.isCanMove) {
            if (_movement != Vector3.zero)
            // _charControll.Move();
            // transform.Rotate(_movement);
        // _charControll.Move(_movement);
            playerRigidbody.MoveRotation(Quaternion.LookRotation(_movement)); 
        playerRigidbody.MovePosition(playerRigidbody.position + _movement);
        }
    }
}
