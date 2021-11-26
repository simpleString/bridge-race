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
        Bulling
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
        StartCoroutine(CheckEnemiesBricks());
    }

    struct PlayerScriptAndDistancePlusTranform {
        public float distance;

        public Transform transform;
        public BasePlayer playerScript;

        public PlayerScriptAndDistancePlusTranform(BasePlayer playerScript, float distance, Transform transform) {
            this.distance = distance;
            this.transform = transform;
            this.playerScript = playerScript;
        }
    }

    private IEnumerator CheckEnemiesBricks() {
        // get all player's brics, and if bricks less that in bot, and distance not far, go to it.
        while (true) {
            while (_currentBotState == BotState.Bulling) {
                yield return new WaitForSeconds(.2f);

            }
            var playersBricks = new List<PlayerScriptAndDistancePlusTranform>();
            PlayerScriptAndDistancePlusTranform? instance = null;

            foreach (var player in GameManager.Instance.players) {
                // TODO:: Check that it's correct way to compare objects
                if (player.gameObject != this.gameObject) {
                    var distance = Vector3.Distance(this.transform.position, player.transform.position);
                    if (distance < GameManager.Instance.enemyBullingThreshold) {
                        Debug.Log("Instace added");
                        playersBricks.Add((new PlayerScriptAndDistancePlusTranform(player, distance, player.transform)));
                    }
                }
            }


            foreach (var item in playersBricks) {
                if ((instance == null && bricks.Count > 2 && item.playerScript.bricks.Count > 0 && bricks.Count > item.playerScript.bricks.Count) ||
                (item.playerScript.bricks.Count > instance?.playerScript.bricks.Count && item.playerScript.bricks.Count < bricks.Count && item.playerScript.bricks.Count > 0)) {
                    Debug.Log("Instace fsfsdfdsfsdf");
                    instance = item;
                }
            }

            if (instance != null) {
                _currentBotState = BotState.Bulling;
                StartCoroutine(EmenyBulling((PlayerScriptAndDistancePlusTranform)instance));
            }

            yield return new WaitForSeconds(.2f);
        }
    }


    IEnumerator EmenyBulling(PlayerScriptAndDistancePlusTranform instance) {
        _currentTarget = instance.transform;
        var timeToBulling = 3f;
        float checkingTime = .2f;
        bool isTrue = true;
        while (isTrue && timeToBulling > 0) {
            yield return new WaitForSeconds(checkingTime);
            timeToBulling -= checkingTime;
            if (instance.playerScript.bricks.Count < bricks.Count &&
            Vector3.Distance(instance.transform.position, transform.position) <= GameManager.Instance.enemyBullingThreshold) {
                _currentTarget = instance.transform;
            } else {
                isTrue = false;
            }
        }
        _currentBotState = BotState.TakeBrick;
        FindNearBrick();
        yield return null;
    }


    IEnumerator CheckRemainingDistanceForBot() {
        while (true) {
            // Check that we a in a last ladder, and switch agent destination to door
            if (_currentTarget == null) {
                FindNearBrick();
            }
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
        } else if (collider.CompareTag("Bonus")) {
            var bonusScript = collider.GetComponent<Bonus>();
            GetBunusEffect(bonusScript.type);
            bonusScript.Destory();
        }
    }

    NearBrick GetNearBrick() {
        NearBrick nBrick = null;
        foreach (var brickScript in GameObject.FindObjectsOfType<Brick>()) {
            if (brickScript.tag == color.ToString() || brickScript.tag == "Free") {
                var tempDistance = Vector3.Distance(transform.position, brickScript.transform.position);
                if (nBrick == null || (tempDistance < nBrick.distance)) {
                    nBrick = new NearBrick
                    {
                        distance = tempDistance,
                        transform = brickScript.transform
                    };
                }

            }
        }
        return nBrick;
    }

    void FindNearBrick() {

        NearBrick nBrick = GetNearBrick();
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
            if (brickScript.tag == color.ToString() || brickScript.tag == "Free") {
                var tempDistance = Vector3.Distance(transform.position, brickScript.transform.position);
                if (nBrick == null || (tempDistance < nBrick.distance)) {
                    nBrick = new NearBrick
                    {
                        distance = tempDistance,
                        transform = brickScript.transform
                    };
                }
            }
        }

        _currentTarget = nBrick.transform;
    }

    void FindBestLadder() {

        var ladders = GameObject.FindObjectsOfType<Ladder>();
        var laddersValues = new List<NearLadder>();

        NearLadder nearLadder = null;
        foreach (var ladder in ladders) {
            var tempNearLadder = new NearLadder(ladder.checkPosition,
                                                Vector3.Distance(ladder.checkPosition.position, transform.position),
                                                ladder.GetCountByColorTag(color),
                                                ladder);
            // Fix it. It cam pick first ladder where already has bricks.
            // TODO:: Fix bots bug when they stop on second floor. It's fixed by set nearLadder to final position!!!
            laddersValues.Add(tempNearLadder);

            Debug.Log("Ladder position " + tempNearLadder.transform.position.y + " bot position: " + transform.position.y);
            if ((nearLadder == null && tempNearLadder.transform.position.y - 2 > transform.position.y) ||
                (tempNearLadder.colorCount >= nearLadder?.colorCount && tempNearLadder.transform.position.y - 2 > transform.position.y && tempNearLadder.transform.position.y <= nearLadder?.transform.position.y)) {

                // if (nearLadder == null || (tempNearLadder.colorCount >= nearLadder.colorCount &&
                //                             tempNearLadder.transform.position.y - 8 > transform.position.y &&
                //                             tempNearLadder.transform.position.y <= nearLadder.transform.position.y)) {
                nearLadder = tempNearLadder;
            }
        }

        // if we see ladder enemy winner enemy, we wanna break  him ladder

        // foreach (var ladder in laddersValues) {
        //     if (ladder.ladderScript.GetCountAnotherColorByTag(color))
        // }


        // Check that ladder distance less that near brick distance
        var nBrick = GetNearBrick();
        Debug.Log("Brick Distance: " + nBrick.distance + " Ladder distance: " + nearLadder.distance);
        if (nBrick != null && nBrick.distance < .5f) {
            _currentBotState = BotState.TakeBrick;
            _currentTarget = nBrick.transform;
        } else {
            _currentTarget = nearLadder.transform;
        }
    }

    //     void FindBestLadder() {

    //     var ladders = GameObject.FindObjectsOfType<Ladder>();
    //     NearLadder nearLadder = null;
    //     foreach (var ladder in ladders) {
    //         var tempNearLadder = new NearLadder(ladder.checkPosition,
    //                                             Vector3.Distance(ladder.checkPosition.position, transform.position),
    //                                             ladder.GetCountByColorTag(color));
    //         // Fix it. It cam pick first ladder where already has bricks.
    //         // TODO:: Fix bots bug when they stop on second floor. It's fixed by set nearLadder to final position!!!
    //         if (nearLadder == null || (tempNearLadder.colorCount >= nearLadder.colorCount &&
    //                                     tempNearLadder.transform.position.y - 1 > transform.position.y &&
    //                                     tempNearLadder.transform.position.y <= nearLadder.transform.position.y)) {
    //             nearLadder = tempNearLadder;
    //         }
    //     }
    //     _currentTarget = nearLadder.transform;
    // }

    class NearBrick {
        public Transform transform;
        public float distance;
    }

    class NearLadder : NearBrick {
        public int colorCount = 0;
        public Ladder ladderScript;

        public NearLadder(Transform transform, float distance, int colorCount, Ladder ladderScript) {
            this.transform = transform;
            this.colorCount = colorCount;
            this.distance = distance;
            this.ladderScript = ladderScript;
        }
    }

}
