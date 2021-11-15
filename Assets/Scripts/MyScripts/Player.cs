using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Player : BasePlayer {

    //////////////////////////////////// Base properties
    [SerializeField] FloatingJoystick _floatingJoystick;

    public static Player Instance = null;
    ///////////////////////////////////




    public float collisionOffset = .1f;


    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Stairs") && !collider.CompareTag(color.ToString())) {
            if (bricks.Count > 0)
                AddBrickToBridge(collider.gameObject);
            else {
                // Don't let player go to stairs
                _agent.Move(collider.gameObject.transform.right * collisionOffset);
            }
        } else if (collider.tag == "Player") {
            Debug.Log("im's here");
            CheckPlayerCollision(collider);
        } else if ((collider.tag == color.ToString() || collider.tag == "Free") && collider.gameObject.layer != LayerMask.NameToLayer("Stairs")) {
            AddBrickToPlayer(collider.gameObject);
        }
    }


    new void Awake() {
        if (Instance != null) {
            Destroy(this);
        }
        Instance = this;

        base.Awake();
    }

    void Start() {
        color = GameManager.Instance.playerColor;
    }


    new void Update() {
#if UNITY_EDITOR
        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");
#else
        var vertical = _floatingJoystick.Vertical;
        var horizontal = _floatingJoystick.Horizontal;
#endif
        _movement = new Vector3(horizontal, 0f, vertical);

        // FIXME:: Needs to optimize for work with bots!!!
        float velocityZ = Vector3.Dot(_movement.normalized, transform.forward);
        float velocityX = Vector3.Dot(_movement.normalized, transform.right);

        _animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
        _animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);
    }

    void FixedUpdate() {
        if (!_agent.enabled) return;
        // Debug.Log(_characterController.transform.position);
        if (_movement.magnitude > 0) {
            _movement.Normalize();
            _movement *= _speed;


            transform.rotation = Quaternion.LookRotation(new Vector3(_movement.x, 0, _movement.z));
        }
        _agent.Move(_movement * Time.deltaTime);
    }

}
