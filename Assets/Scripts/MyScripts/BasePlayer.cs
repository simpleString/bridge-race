using System.Collections.Generic;
using UnityEngine;

public class BasePlayer: MonoBehaviour {

    [SerializeField] protected float _speed = 3;
    public System.Action playerLostBrick;
    public bool isCanMove = true;
    public Transform BrickHolder;
    public Transform PortableBrickPrefab;

    public GameManager.MyColor myColor; 

    public float collisionOffset = .1f; // Offset for climbing

    protected Vector3 movement;

    protected Animator _animator;
    protected void Awake()
    {
        
        _animator = GetComponent<Animator>();
    }

    protected Stack<Transform> _countOfBricks = new Stack<Transform>();

    

    protected void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == myColor.ToString()) {
            AddBrickToPlayer();
            var brick = collider.gameObject.GetComponent<Brick>();
            brick.Destroy();
            Destroy(collider.gameObject);
        }
    }

    protected void OnCollisionEnter(Collision collision) {
       if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Stairs")) {
           if (collision.gameObject.CompareTag(myColor.ToString())) { // FIXME:: Update to work with bots
           } else {
                if (_countOfBricks.Count > 0)
                    AddBrickToBridge(collision.gameObject);
                else  {
                    // Don't let player go to stairs
                    GetComponent<Rigidbody>().MovePosition(transform.position + collision.gameObject.transform.right * collisionOffset);
                }
           }

       }
    }

    protected void AddBrickToPlayer() {
        var newPortableBrick = Instantiate(PortableBrickPrefab, new Vector3(
            BrickHolder.position.x,
            BrickHolder.position.y + PortableBrickPrefab.GetComponent<Renderer>().bounds.size.y * _countOfBricks.Count, // FIXME:: optimize this shit
            BrickHolder.position.z
        ), Quaternion.Euler(Vector3.down));
        newPortableBrick.tag = MyConstants.TagNull; // it's save us from bugs, but it's not required
        newPortableBrick.GetComponent<Renderer>().material.color = GameManager.GetUnityColorByMyColor(myColor);
        newPortableBrick.parent = BrickHolder;
        _countOfBricks.Push(newPortableBrick);
    }


    protected void AddBrickToBridge(GameObject brick) {
        var brickStript = brick.GetComponent<Stair>();
        brickStript.ChangeColor(myColor);
        Destroy(_countOfBricks.Pop().gameObject);
        playerLostBrick?.Invoke();
    }
}