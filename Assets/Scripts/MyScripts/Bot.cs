using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : BasePlayer {
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

    void Start() {
        StartCoroutine(CheckRemainingDistanceForBot());
        _agent.speed = 6f;
        FindNearBrick();

        if (_currentBrickTarger.transform != null) {
            _agent.destination = _currentBrickTarger.transform.position;
        }
    }

    IEnumerator CheckRemainingDistanceForBot() {
        while (true) {
            // Check that we a in a last ladder, and switch agent destination to door
            if (_currentBotState == BotState.TakeLadder && _agent.remainingDistance < 0.2f && _agent.destination != movePositionTransform.position)
                FindBestLadder();
            yield return new WaitForSeconds(.4f);
        }
    }

    // IEnumerator HardStop()
    // {
    //     GetComponent<Collider>().isTrigger = false;
    //     GetComponent<Rigidbody>().MovePosition(transform.position);
    //     yield return new WaitForSeconds(1f);
    //     GetComponent<Collider>().isTrigger = true;
    // }

    new void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Stairs")) {
            if (!collider.gameObject.CompareTag(myColor.ToString())) {
                if (_countOfBricks.Count > 0) {
                    AddBrickToBridge(collider.gameObject);
                } else {
                    _agent.velocity = Vector3.zero;
                }
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
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Stairs") && !collision.gameObject.CompareTag(myColor.ToString())) {
            if (_countOfBricks.Count > 0)
                AddBrickToBridge(collision.gameObject);
            else {
                // Don't let player go to stairs
                // GetComponent<Rigidbody>().MovePosition(transform.position + collision.gameObject.transform.right * collisionOffset);
            }
        }
    }

    public void Init(GameManager.MyColor color) {
        myColor = color;
    }

    void FindNearBrick() {

        NearBrick nBrick = new NearBrick();
        // TODO:: Needs to optimize it. Now it return all objects from all platforms
        foreach (var brick in GameObject.FindGameObjectsWithTag(myColor.ToString())) { // Find near brick from all bricks with same tag
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
        foreach (var brick in GameObject.FindGameObjectsWithTag(myColor.ToString())) { // Find near brick from all bricks with same tag
            if (brick.GetComponent<Stair>() != null) continue; // I fuck this shit!!!!
            if (brick == exceptBrick) continue;
            // if (transform.position.y < brick.transform.position.y) continue;
            var tempDistance = Vector3.Distance(transform.position, brick.transform.position);
            if (tempDistance < nBrick.distance) {
                nBrick.distance = tempDistance;
                nBrick.transform = brick.transform;
            }
        }

        _currentBrickTarger = nBrick;
    }

    void FindBestLadder() {

        var ladders = GameObject.FindObjectsOfType<Ladder>();
        NearLadder nearLadder = null;
        // if (ladders != null)
        // {
        //     nearLadder = new NearLadder(ladders[0].checkPosition,
        //                                 Vector3.Distance(transform.position, ladders[0].checkPosition.position),
        //                                 ladders[0].GetCountByColorTag(tag));
        // }
        foreach (var ladder in ladders) {
            Debug.Log("Color count: " + ladder.GetCountByColorTag(myColor));
            var tempNearLadder = new NearLadder(ladder.checkPosition,
                                                Vector3.Distance(ladder.checkPosition.position, transform.position),
                                                ladder.GetCountByColorTag(myColor));
            if (nearLadder == null || (tempNearLadder.colorCount >= nearLadder.colorCount &&
                                        tempNearLadder.transform.position.y > transform.position.y)) {
                nearLadder = tempNearLadder;
            }
        }
        _agent.destination = nearLadder.transform.position;
    }

    class NearBrick {
        public Transform transform;
        public float distance = 999999999f;
    }

    class NearLadder : NearBrick {
        public int colorCount = 0;

        public NearLadder(Transform transform, float distance, int colorCount) {
            this.transform = transform;
            this.colorCount = colorCount;
            this.distance = distance;
        }
    }

}
