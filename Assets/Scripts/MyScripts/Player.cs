using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    [SerializeField] float _speed = 3;
    [SerializeField] FloatingJoystick _floatingJoystick;
    private PlayerController _playerController;

    public bool isCanMove = true;
    public Vector3 stopVector;
    
    public Transform BrickHolder;
    public Transform PortableBrickPrefab;
    public Color color;
    public float collisionOffset = .1f;

    Vector3 movement;
    
    int currentPortableBricksCount = 0;

    Animator _animator;

    string colorName = "red";
    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();
    }

    Stack<Transform> _playerBricks = new Stack<Transform>();

    void Update()
    {
    #if UNITY_EDITOR
        var vertical = Input.GetAxis ("Vertical");
        var horizontal = Input.GetAxis ("Horizontal"); 
        

    #else
        var vertical = _floatingJoystick.Vertical;
        var horizontal = _floatingJoystick.Horizontal;
    #endif
        movement = new Vector3(horizontal, 0f, vertical);
        if (movement.magnitude > 0)
        {
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
        if (collider.gameObject.tag == colorName) {
            AddBrickToPlayer(collider.transform);
            var brick = collider.gameObject.GetComponent<Brick>();
            brick.Destroy();
            Destroy(collider.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision) {
       if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Stairs")) {
           if (collision.gameObject.CompareTag(colorName)) {

           } else {
                if (_playerBricks.Count > 0)
                    AddBrickToBridge(collision.gameObject);
                else  {
                    GetComponent<Rigidbody>().MovePosition(transform.position + collision.gameObject.transform.right * collisionOffset);
                }
           }

       }
    }

    void AddBrickToPlayer(Transform instance) {
        var newPortableBrick = Instantiate(instance, new Vector3(
            BrickHolder.position.x - PortableBrickPrefab.localScale.x,
            PortableBrickPrefab.localScale.y * _playerBricks.Count + 1,
            BrickHolder.position.z
        ), Quaternion.Euler(Vector3.down));
        newPortableBrick.parent = BrickHolder;
        _playerBricks.Push(newPortableBrick);
    }


    void AddBrickToBridge(GameObject brick) {
        brick.tag = colorName;
        Color setColor;
        switch (colorName)
        {
            case "red":
                setColor = Color.red;
                brick.tag = "red";
                break;
            case "green":
                setColor = Color.green;
                brick.tag = "green";
                break;
            case "blue":
                setColor = Color.blue;
                brick.tag = "blue";
                break;
            default:
                setColor = Color.black;
                brick.tag = "black";
                break;
        }
        var renderer = brick.GetComponent<Renderer>();
        renderer.material.color = setColor;
        renderer.enabled = true;
        Destroy(_playerBricks.Pop().gameObject);
    }
}
