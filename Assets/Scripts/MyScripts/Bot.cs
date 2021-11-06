using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : BasePlayer
{
    public Transform movePositionTransform;
    NavMeshAgent _agent;

    enum BotState {
        TakeBrick,
        TakeLadder,
        Idle,
    }

    BotState _currentBotState = BotState.TakeBrick;

    private NearBrick _currentBrickTarger;
    new void Awake() {
        playerLostBrick += OnBotLostBrick;
        _agent = GetComponent<NavMeshAgent>();
    }

    void OnBotLostBrick() {
        Debug.Log("hello, current count is : " + _countOfBricks.Count);
        if (_countOfBricks.Count < 1) {
            Debug.Log("took, took");
            _currentBotState = BotState.TakeBrick;
            FindNearBrick();
            Debug.Log("new Destination: " + _currentBrickTarger.distance);
            _agent.destination = _currentBrickTarger.transform.position;
        }
    }

    void Start()
    {
        _agent.speed = 6f;
        FindNearBrick();
        
        if (_currentBrickTarger.transform != null) {
            _agent.destination = _currentBrickTarger.transform.position;
        }
    }

    IEnumerator HardStop() {
        GetComponent<Collider>().isTrigger = false;
        GetComponent<Rigidbody>().MovePosition(transform.position);
        yield return new WaitForSeconds(1f);
        GetComponent<Collider>().isTrigger = true; 
    }

    new void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Stairs")) {
            if (!collider.gameObject.CompareTag(myColor.ToString())) { // FIXME:: Update to work with bots
                if (_countOfBricks.Count > 0) {
                    AddBrickToBridge(collider.gameObject);
                } else {
                    StartCoroutine(HardStop());
                    GetComponent<Rigidbody>().MovePosition(transform.position + collider.gameObject.transform.right * collisionOffset * _agent.speed);
                    // GetComponent<Collider>().isTrigger = false;
                } 
            } else {
                // GetComponent<Collider>().isTrigger = true;  
            }
        } else {
            if (collider.gameObject.tag == myColor.ToString()) {
                if (_countOfBricks.Count > 5) {
                    _currentBotState = BotState.TakeLadder;
                    FindBestLadder();
                } else {
                    FindNearBrick(collider.gameObject);
                    Debug.Log("new Destination: " + _currentBrickTarger.distance);
                    _agent.destination = _currentBrickTarger.transform.position;
                }
            }
            base.OnTriggerEnter(collider);
        }
    }

    new void OnCollisionEnter(Collision collision) {
       if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Stairs")) {
           if (collision.gameObject.CompareTag(myColor.ToString())) { // FIXME:: Update to work with bots
           } else {
                if (_countOfBricks.Count > 0)
                    AddBrickToBridge(collision.gameObject);
                else  {
                    // Don't let player go to stairs
                    // GetComponent<Rigidbody>().MovePosition(transform.position + collision.gameObject.transform.right * collisionOffset);
                }
           }

       }
    }

    public void Init(GameManager.MyColor color) {
        myColor = color;
    }

    void FindNearBrick() {
        
        NearBrick nBrick = new NearBrick();
        // TODO:: Needs to optimize it. Now it return all objects from all platforms
        foreach(var brick in GameObject.FindGameObjectsWithTag(myColor.ToString())) { // Find near brick from all bricks with same tag
            if (brick.GetComponent<Stair>() != null) continue;
            if (transform.position.y < brick.transform.position.y) continue;
            var tempDistance = Vector3.Distance(new Vector3(transform.position.x, transform.position.y, transform.position.z), brick.transform.position);
            if (tempDistance < nBrick.distance) {
                nBrick.distance = tempDistance;
                nBrick.transform = brick.transform;
            }
        }
        _currentBrickTarger = nBrick;
    }

    void FindNearBrick(GameObject exceptBrick) { // it fixed bug when it stay on taked brick
        
        NearBrick nBrick = new NearBrick();
        // TODO:: Needs to optimize it. Now it return all objects from all platforms
        foreach(var brick in GameObject.FindGameObjectsWithTag(myColor.ToString())) { // Find near brick from all bricks with same tag
            if (brick.GetComponent<Stair>() != null) continue; // I fuck this shit!!!!
            if (brick == exceptBrick) continue;
            if (transform.position.y < brick.transform.position.y) continue;
            var tempDistance = Vector3.Distance(transform.position, brick.transform.position);
            if (tempDistance < nBrick.distance) {
                nBrick.distance = tempDistance;
                nBrick.transform = brick.transform;
            }
        }
        
        _currentBrickTarger = nBrick;
    }

    void FindBestLadder() {
        _agent.destination = movePositionTransform.position;
    }

    class NearBrick
    {
        public Transform transform;
        public float distance = 999999999f;
    }

}
