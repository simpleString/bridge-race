using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player1 : MonoBehaviour {
    [SerializeField] float _speed = 3;
    [SerializeField] FloatingJoystick _floatingJoystick;
    private PlayerController _playerController;

    public System.Action playerLostBrick;
    public bool isCanMove = true;
    public Transform BrickHolder;
    public Transform PortableBrickPrefab;
    public float collisionOffset = .1f; // Offset for climbing

    Vector3 movement;

    Animator _animator;
    void Awake() {
        _playerController = GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();
    }

    Stack<Transform> _countOfBricks = new Stack<Transform>();

    void Update() {
#if UNITY_EDITOR
        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");


#else
        var vertical = _floatingJoystick.Vertical;
        var horizontal = _floatingJoystick.Horizontal;
#endif
        movement = new Vector3(horizontal, 0f, vertical);
        if (movement.magnitude > 0) {
            movement.Normalize();
            movement *= _speed * Time.fixedDeltaTime;

        }
        _playerController.Move(movement);

        float velocityZ = Vector3.Dot(movement.normalized, transform.forward);
        float velocityX = Vector3.Dot(movement.normalized, transform.right);

        _animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
        _animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == GameManager.Instance.playerColor.ToString()) {
            AddBrickToPlayer();
            var brick = collider.gameObject.GetComponent<Brick>();
            brick.Destroy();
            Destroy(collider.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Stairs")) {
            if (collision.gameObject.CompareTag(GameManager.Instance.playerColor.ToString())) { // FIXME:: Update to work with bots
            } else {
                if (_countOfBricks.Count > 0)
                    AddBrickToBridge(collision.gameObject);
                else {
                    // Don't let player go to stairs
                    GetComponent<Rigidbody>().MovePosition(transform.position + collision.gameObject.transform.right * collisionOffset);
                }
            }

        }
    }

    void AddBrickToPlayer() {
        var newPortableBrick = Instantiate(PortableBrickPrefab, new Vector3(
            BrickHolder.position.x,
            BrickHolder.position.y + PortableBrickPrefab.GetComponent<Renderer>().bounds.size.y * _countOfBricks.Count, // FIXME:: optimize this shit
            BrickHolder.position.z
        ), Quaternion.Euler(Vector3.down));
        newPortableBrick.tag = MyConstants.TagNull; // it's save us from bugs, but it's not required
        newPortableBrick.GetComponent<Renderer>().material.color = GameManager.GetUnityColorByMyColor(GameManager.Instance.playerColor);
        newPortableBrick.parent = BrickHolder;
        _countOfBricks.Push(newPortableBrick);
    }


    void AddBrickToBridge(GameObject brick) {
        var brickStript = brick.GetComponent<Stair>();
        brickStript.ChangeColor(GameManager.Instance.playerColor);
        Destroy(_countOfBricks.Pop().gameObject);
        playerLostBrick?.Invoke();
    }
}
