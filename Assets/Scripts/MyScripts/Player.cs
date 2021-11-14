using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Player : BasePlayer {

    //////////////////////////////////// Base properties
    [SerializeField] FloatingJoystick _floatingJoystick;

    public static Player Instance = null;
    ///////////////////////////////////


    public float brickForce = 100f;

    public float collisionOffset = .1f;


    protected void CheckPlayerCollision(Collider collider) {
        var otherBasePlayerScript = collider.GetComponent<BasePlayer>();
        if (otherBasePlayerScript.bricks.Count > bricks.Count) {
            DropBricks();
        }
    }

    private void DropBricks() {
        foreach (var brick in bricks) {
            Debug.Log("i'm here");
            brick.GetComponent<Brick>().Init(GameManager.MyColor.black, 0, 0);
            brick.GetComponent<Collider>().isTrigger = false;
            brick.tag = "test";
            brick.transform.parent = null;
            var rb = brick.gameObject.AddComponent<Rigidbody>();
            rb.AddForce(new Vector3(Random.Range(-2f, 3f), Random.Range(0, 3f), Random.Range(-2f, 3f)) * brickForce);
            actionPlayerLostBrick?.Invoke(color);
        }
        bricks.Clear();

    }


    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Stairs") && !collider.CompareTag(color.ToString())) {
            if (bricks.Count > 0)
                AddBrickToBridge(collider.gameObject);
            else {
                // Don't let player go to stairs
                _agent.Move(collider.gameObject.transform.right * collisionOffset);
            }
        } else if (collider.tag == "Player") {
            CheckPlayerCollision(collider);
        } else if ((collider.tag == color.ToString() || collider.tag == "Free") && collider.gameObject.layer != LayerMask.NameToLayer("Stairs")) {
            AddBrickToPlayer();
            var brick = collider.gameObject.GetComponent<Brick>();
            brick.Destroy();
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

        // Debug.Log(_characterController.transform.position);
        if (_movement.magnitude > 0) {
            _movement.Normalize();
            _movement *= _speed;


            transform.rotation = Quaternion.LookRotation(new Vector3(_movement.x, 0, _movement.z));
        }
        _agent.Move(_movement * Time.deltaTime);
    }

}
