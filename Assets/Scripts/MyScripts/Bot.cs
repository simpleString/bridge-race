using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : BasePlayer {

    public int botBricksThreshold = 5;

    private bool _isFirstStart = true;

    enum BotState {
        TakeBrick,
        TakeLadder,
        Idle,
    }
    BotState _currentBotState = BotState.TakeBrick;


    private Transform _currentTarget;


    new void Awake() {
        actionPlayerLostBrick += OnBotLostBrick;
        base.Awake();
    }

    new void Update() {
        base.Update();

        // transform.rotation = Quaternion.LookRotation(new Vector3())
    }

    void FixedUpdate() {

        // if (_movement.magnitude > 0) {
        //     _movement.Normalize();
        //     _movement *= _speed;
        //     transform.rotation = Quaternion.LookRotation(new Vector3(_movement.x, 0, _movement.z));
        // }
        // _agent.Move(_movement * Time.deltaTime);
    }

    void OnBotLostBrick(GameManager.MyColor color) {
        if (bricks.Count < 1) {
            _currentBotState = BotState.TakeBrick;
            FindNearBrick();
        }
    }

    void Start() {
        _currentBotState = BotState.TakeBrick;
        StartCoroutine(FirstStart());
        StartCoroutine(CheckRemainingDistanceForBot());
        StartCoroutine(UpdateDestination());
    }

    IEnumerator CheckRemainingDistanceForBot() {
        while (true) {
            // Check that we a in a last ladder, and switch agent destination to door
            if (_agent.enabled && _currentBotState == BotState.TakeLadder && _agent.remainingDistance < 0.2f) {
                FindBestLadder();
            }
            yield return new WaitForSeconds(.4f);
        }
    }


    IEnumerator UpdateDestination() {
        while (true) {
            if (_agent.enabled && _currentTarget != null) {

                _agent.destination = _currentTarget.position;
                // transform.rotation = Quaternion.LookRotation(new Vector3(_currentTarget.position.x, 0, _currentTarget.position.z));
            }
            yield return new WaitForSeconds(.4f);
        }
    }


    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Stairs") && !collider.CompareTag(color.ToString())) {
            if (bricks.Count > 0)
                AddBrickToBridge(collider.gameObject);
            else {
                // Don't let player go to stairs
                _agent.velocity = Vector3.zero;
                // _agent.Move(collider.gameObject.transform.right * collisionOffset);
            }
        } else if (collider.tag == "Player") {
            CheckPlayerCollision(collider);
            if (bricks.Count < 1) {
                FindNearBrick();
            }
        } else if ((collider.tag == color.ToString() || collider.tag == "Free") && collider.gameObject.layer != LayerMask.NameToLayer("Stairs")) {
            AddBrickToPlayer(collider.gameObject);
            if (bricks.Count > botBricksThreshold) {
                _currentBotState = BotState.TakeLadder;
                FindBestLadder();
            } else {
                FindNearBrick(collider.gameObject);
            }
        }
    }


    void FindNearBrick() {
        NearBrick nBrick = null;
        foreach (var brickScript in GameObject.FindObjectsOfType<Brick>()) {
            if (brickScript.tag != color.ToString()) continue;
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
            if (brickScript.tag != color.ToString()) continue;
            var tempDistance = Vector3.Distance(transform.position, brickScript.transform.position);
            if (nBrick == null || (tempDistance < nBrick.distance)) {
                nBrick = new NearBrick
                {
                    distance = tempDistance,
                    transform = brickScript.transform
                };
            }

        }

        _currentTarget = nBrick.transform;
    }

    void FindBestLadder() {

        var ladders = GameObject.FindObjectsOfType<Ladder>();
        NearLadder nearLadder = null;
        foreach (var ladder in ladders) {
            var tempNearLadder = new NearLadder(ladder.checkPosition,
                                                Vector3.Distance(ladder.checkPosition.position, transform.position),
                                                ladder.GetCountByColorTag(color));
            Debug.Log("Ladder position " + tempNearLadder.transform.position);
            Debug.Log("Bot Position " + transform.position.y); // Fix it. It cam pick first ladder where already has bricks.
            // TODO:: Fix bots bug when they stop on second floor. It's fixed by set nearLadder to final position!!!
            if (nearLadder == null || (tempNearLadder.colorCount >= nearLadder.colorCount &&
                                        tempNearLadder.transform.position.y - 1 > transform.position.y &&
                                        tempNearLadder.transform.position.y <= nearLadder.transform.position.y)) {
                nearLadder = tempNearLadder;
            }
        }
        _currentTarget = nearLadder.transform;
    }

    class NearBrick {
        public Transform transform;
        public float distance;
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
