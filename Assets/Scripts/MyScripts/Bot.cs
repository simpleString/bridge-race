using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : BasePlayer {
    public Transform movePositionTransform;
    NavMeshAgent _agent;
    private Rigidbody _rb;

    private bool _isFirstStart = true;

    enum BotState {
        TakeBrick,
        TakeLadder,
        Idle,
    }
    BotState _currentBotState = BotState.TakeBrick;

    private NearBrick _currentBrickTarget;

    private Transform _currentTarget;
    new void Awake() {
        base.Awake();
        playerLostBrick += OnBotLostBrick;
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _agent.speed = 6f;
    }

    void Update() {

        // if (_agent.velocity != Vector3.zero)
        _rb.MoveRotation(Quaternion.LookRotation(_agent.velocity.normalized));



        float velocityZ = Vector3.Dot(_agent.velocity.normalized, transform.forward);
        float velocityX = Vector3.Dot(_agent.velocity.normalized, transform.right);

        _animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
        _animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);
    }

    void OnBotLostBrick(GameManager.MyColor color) {
        if (countOfBricks.Count < 1) {
            _currentBotState = BotState.TakeBrick;
            FindNearBrick();
        }
    }


    IEnumerator CheckRemainingDistanceForBot() {
        while (true) {
            // Check that we a in a last ladder, and switch agent destination to door
            if (_currentBotState == BotState.TakeLadder && _agent.remainingDistance < 0.2f && _agent.destination != movePositionTransform.position) {
                FindBestLadder();
            }
            yield return new WaitForSeconds(.4f);
        }
    }


    void UpdateDestination() {
        _agent.destination = _currentTarget.position;
    }



    new void OnTriggerEnter(Collider collider) {
        // Debug.Log("Collision detected: " + collider.tag);
        if (collider.gameObject.layer == LayerMask.NameToLayer("Stairs")) {
            if (!collider.gameObject.CompareTag(myColor.ToString())) {
                if (countOfBricks.Count > 0) {
                    AddBrickToBridge(collider.gameObject);
                } else {
                    _agent.velocity = Vector3.zero;
                }
            }
        } else {
            if (collider.gameObject.tag == myColor.ToString()) {
                if (countOfBricks.Count > 5) {
                    _currentBotState = BotState.TakeLadder;
                    FindBestLadder();
                } else {
                    FindNearBrick(collider.gameObject);
                }
            }
            base.OnTriggerEnter(collider);
        }
    }

    new void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Stairs") && !collision.gameObject.CompareTag(myColor.ToString())) {
            if (countOfBricks.Count > 0)
                AddBrickToBridge(collision.gameObject);
        }
    }

    public void Init(GameManager.MyColor color) {
        myColor = color;
        StartCoroutine(FirstStart());
        // FindNearBrick();

    }

    void FindNearBrick() {

        NearBrick nBrick = null;
        Debug.Log("CurrentCount: " + GameObject.FindObjectsOfType<Brick>().Length);
        foreach (var brickScript in GameObject.FindObjectsOfType<Brick>()) {
            if (brickScript.tag != myColor.ToString()) continue;
            // if (transform.position.y < brickScript.transform.position.y) continue;
            var tempDistance = Vector3.Distance(transform.position, brickScript.transform.position);
            if (nBrick == null || (tempDistance < nBrick.distance)) {
                nBrick = new NearBrick
                {
                    distance = tempDistance,
                    transform = brickScript.transform
                };
            }

        }
        if (nBrick != null) {

            _currentTarget = nBrick.transform;
            Debug.Log("_current Targer is: " + nBrick.transform);
            UpdateDestination();
        }
    }

    IEnumerator FirstStart() {
        while (_isFirstStart) {
            FindNearBrick();
            Debug.Log("current dtate");
            if (_currentTarget != null) {
                _isFirstStart = false;
            }
            yield return new WaitForSeconds(.1f);
        }
        yield return null;
    }

    void FindNearBrick(GameObject exceptBrick) { // it fixed bug when it stay on taked brick

        NearBrick nBrick = null;

        foreach (var brickScript in GameObject.FindObjectsOfType<Brick>()) {
            if (brickScript.gameObject == exceptBrick) continue;
            if (brickScript.tag != myColor.ToString()) continue;
            // if (transform.position.y < brickScript.transform.position.y) continue;
            var tempDistance = Vector3.Distance(transform.position, brickScript.transform.position);
            if (nBrick == null || (tempDistance < nBrick.distance)) {
                nBrick = new NearBrick
                {
                    distance = tempDistance,
                    transform = brickScript.transform
                };
            }

        }


        // foreach (var brick in GameObject.FindGameObjectsWithTag(myColor.ToString())) { // Find near brick from all bricks with same tag
        //     if (brick.GetComponent<Stair>() != null) continue; // I fuck this shit!!!!
        //     if (brick == exceptBrick) continue;
        //     // if (transform.position.y < brick.transform.position.y) continue;
        //     var tempDistance = Vector3.Distance(transform.position, brick.transform.position);
        //     if (tempDistance < nBrick.distance) {
        //         nBrick.distance = tempDistance;
        //         nBrick.transform = brick.transform;
        //     }
        // }

        _currentTarget = nBrick.transform;
        UpdateDestination();
    }

    void FindBestLadder() {

        var ladders = GameObject.FindObjectsOfType<Ladder>();
        NearLadder nearLadder = null;
        foreach (var ladder in ladders) {
            // Debug.Log("Color count: " + ladder.GetCountByColorTag(myColor));
            var tempNearLadder = new NearLadder(ladder.checkPosition,
                                                Vector3.Distance(ladder.checkPosition.position, transform.position),
                                                ladder.GetCountByColorTag(myColor));
            if (nearLadder == null || (tempNearLadder.colorCount >= nearLadder.colorCount &&
                                        tempNearLadder.transform.position.y > transform.position.y)) {
                nearLadder = tempNearLadder;
            }
        }
        // transform.LookAt(nearLadder.transform.position);
        // _agent.destination = nearLadder.transform.position;
        _currentTarget = nearLadder.transform;
        UpdateDestination();
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
