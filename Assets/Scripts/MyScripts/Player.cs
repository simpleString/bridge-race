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

    Queue<Transform> _playerBricks = new Queue<Transform>();

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
        // Debug.Log("name: " + collider.gameObject.name); 
        // Debug.Log("Layer: " + collider.gameObject.layer);
        // if (collider.gameObject.layer == LayerMask.NameToLayer("Stairs")) {
        //     Debug.Log("Collide with a stair");
        // }
    }

    void OnCollisionEnter(Collision collision) {
        // Debug.Log(collision.gameObject.transform.tag);
       if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Stairs")) {
           if (collision.gameObject.CompareTag(colorName)) {
                // Debug.Log(collision.gameObject.tag);

           } else {


                // Debug.Log(collision.gameObject.tag);
                if (_playerBricks.Count > 0)
                    AddBrickToBridge(collision.gameObject);
                else  {
                    // stopVector = transform.forward;
                    // stopVector.Normalize();
                    // Debug.Log("colliderPosition: " + collision.gameObject.transform.position);
                    Debug.Log("colliderPositionForward: " + collision.gameObject.transform.forward);
                    Debug.Log("colliderPositionRight: " + collision.gameObject.transform.right);
                    // Debug.Log("colliderPositionRoot: " + collision.gameObject.transform.root);
                    // Debug.Log("postion: " + collision.transform.localToWorldMatrix);
                    // Debug.Log("normolize: " + stopVector);
                    // var tempMovement = transform;
                    // Debug.Log("PlayerMovement: " + tempMovement);
                    GetComponent<Rigidbody>().MovePosition(transform.position + collision.gameObject.transform.right * collisionOffset);
                    // isCanMove = false;
                }
           }

       }
        // for(var collider = 0; collider < collision.contactCount; collider++) {
        //     Debug.Log("Collider: " + collision.contacts[collider].thisCollider.gameObject.name);
        // }
    }

    void AddBrickToPlayer(Transform instance) {
        var newPortableBrick = Instantiate(instance, new Vector3(
            BrickHolder.position.x - PortableBrickPrefab.localScale.x,
            PortableBrickPrefab.localScale.y * _playerBricks.Count + 1,
            BrickHolder.position.z
        ), Quaternion.Euler(Vector3.down));
        newPortableBrick.parent = BrickHolder;
        _playerBricks.Enqueue(newPortableBrick);
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
        Destroy(_playerBricks.Dequeue().gameObject);
    }
}
