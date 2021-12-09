using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : BasePlayer {

    public int botBricksThreshold = 5;

    private bool _isFirstStart = true;

    public enum BotState {
        Idle,
        TakeBrick,
        TakeLadder,
        Bulling,
        Escape,
    }
    public BotState _currentBotState = BotState.TakeBrick;


    private Transform _currentTarget;


    new void Awake() {
        // actionPlayerLostBrick += OnBotLostBrick;
        base.Awake();
        _agent.angularSpeed = 2000f;
        _currentBotState = BotState.Idle;
    }

    //Control bulling state
    private bool isBulling = false;
    private PlayerScriptAndDistancePlusTranform targetPlayer = null;

    //Control state when bot stay on brick and don't pick it
    private Vector3 _botPosition;
    private bool _checkStayBrick = false;

    public float multiplyBy; // For bot excaping

    IEnumerator StateMachine() {
        while (true) {
            NearBrick nearBrickTarget = null;
            BestLadder bestLadderTarget = null;


            var nearPlayer = GetNearPlayerWithBricksMoreThatMe();
            // antiBulling checking
            if (nearPlayer != null && _agent.enabled) {
                transform.rotation = Quaternion.LookRotation(transform.position - nearPlayer.player.transform.position);
                Vector3 runTo = transform.position + transform.forward * multiplyBy;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(runTo, out hit, 5, NavMesh.AllAreas)) {
                    _agent.SetDestination(hit.position);
                }
            }

            switch (_currentBotState) {
                case BotState.Idle:
                    //TODO:: Now it check only one state. Needs add more
                    nearBrickTarget = GetNearBrick();
                    if (nearBrickTarget != null) {
                        _currentTarget = nearBrickTarget.transform;
                        _currentBotState = BotState.TakeBrick;
                    }
                    break;

                case BotState.TakeLadder:
                    // Check that bot has bricks
                    if (bricks.Count < 2) {
                        _currentBotState = BotState.TakeBrick;
                    }
                    bestLadderTarget = GetBestLadder();

                    _currentTarget = bestLadderTarget.transform;
                    break;

                case BotState.TakeBrick:

                    // TODO:: Check bulling transaction
                    // targetPlayer = CheckEnemiesBricks();
                    // if (targetPlayer != null) {

                    //     isBulling = true;
                    //     _currentBotState = BotState.Bulling;
                    // }

                    // Check that bot has enouth bricks
                    if (bricks.Count > botBricksThreshold) {
                        // Check that near hasn't any bricks
                        nearBrickTarget = GetNearBrick();
                        if (nearBrickTarget != null && nearBrickTarget.distance < .5f) {
                            _currentTarget = nearBrickTarget.transform;
                        } else {
                            // If not blocks find best ladder
                            _currentBotState = BotState.TakeLadder;
                        }
                    } else {
                        nearBrickTarget = GetNearBrick();
                        if (nearBrickTarget != null) {
                            _currentTarget = nearBrickTarget.transform;
                        } // Do nothing
                        else if (bricks.Count > 1) { // If in spawner ends bricks, go to ladder
                            _currentBotState = BotState.TakeLadder;
                        } else {
                            _currentTarget = null;
                        }
                    }
                    break;
                case BotState.Bulling:
                    if (isBulling) {
                        //         if (targetPlayer.playerScript.bricks.Count < bricks.Count && targetPlayer.playerScript.bricks.Count > 0 &&
                        // Vector3.Distance(targetPlayer.transform.position, transform.position) <= GameManager.Instance.enemyBullingThreshold) {
                        //             _currentTarget = targetPlayer.transform;
                        if (targetPlayer.playerScript.bricks.Count < bricks.Count && targetPlayer.playerScript.bricks.Count > 0) {
                        } else {
                            targetPlayer = null;
                            isBulling = false;
                            _currentBotState = BotState.Idle;
                            StopCoroutine(TimeToEnemyBulling());
                        }
                    } else {
                        StartCoroutine(TimeToEnemyBulling());
                    }

                    break;
                    // // if already doing bulling
                    // if (bullingCoroutineStatus != null) return;
                    // targetPlayer = CheckEnemiesBricks();
                    // if (targetPlayer == null) _currentBotState = BotState.Idle;
                    // bullingCoroutineStatus = StartCoroutine(EnemyBulling(targetPlayer));
                    // if (bullingCoroutineStatus == null) {
                    //     _currentBotState = BotState.Idle;
                    // }
                    // break;
            }

            yield return new WaitForSeconds(.1f);
        }
    }


    new void Update() {
        base.Update();



        // NearBrick nearBrickTarget = null;
        // BestLadder bestLadderTarget = null;


        // var nearPlayer = GetNearPlayerWithBricksMoreThatMe();
        // // antiBulling checking
        // if (nearPlayer != null) {

        // }

        // switch (_currentBotState) {
        //     case BotState.Idle:
        //         //TODO:: Now it check only one state. Needs add more
        //         nearBrickTarget = GetNearBrick();
        //         if (nearBrickTarget != null) {
        //             _currentTarget = nearBrickTarget.transform;
        //             _currentBotState = BotState.TakeBrick;
        //         }
        //         break;

        //     case BotState.TakeLadder:
        //         // Check that bot has bricks
        //         if (bricks.Count < 2) {
        //             _currentBotState = BotState.TakeBrick;
        //         }
        //         bestLadderTarget = GetBestLadder();

        //         _currentTarget = bestLadderTarget.transform;
        //         break;

        //     case BotState.TakeBrick:

        //         // TODO:: Check bulling transaction
        //         targetPlayer = CheckEnemiesBricks();
        //         if (targetPlayer != null) {

        //             isBulling = true;
        //             _currentBotState = BotState.Bulling;
        //         }

        //         // Check that bot has enouth bricks
        //         if (bricks.Count > botBricksThreshold) {
        //             // Check that near hasn't any bricks
        //             nearBrickTarget = GetNearBrick();
        //             if (nearBrickTarget != null && nearBrickTarget.distance < .5f) {
        //                 _currentTarget = nearBrickTarget.transform;
        //             } else {
        //                 // If not blocks find best ladder
        //                 _currentBotState = BotState.TakeLadder;
        //             }
        //         } else {
        //             nearBrickTarget = GetNearBrick();
        //             if (nearBrickTarget != null) {
        //                 _currentTarget = nearBrickTarget.transform;
        //             } // Do nothing
        //             else if (bricks.Count > 1) { // If in spawner ends bricks, go to ladder
        //                 _currentBotState = BotState.TakeLadder;
        //             } else {
        //                 _currentTarget = null;
        //             }
        //         }
        //         break;
        //     case BotState.Bulling:
        //         if (isBulling) {
        //             if (targetPlayer.playerScript.bricks.Count < bricks.Count && targetPlayer.playerScript.bricks.Count > 0 &&
        //     Vector3.Distance(targetPlayer.transform.position, transform.position) <= GameManager.Instance.enemyBullingThreshold) {
        //                 _currentTarget = targetPlayer.transform;
        //             } else {
        //                 targetPlayer = null;
        //                 isBulling = false;
        //                 _currentBotState = BotState.Idle;
        //                 StopCoroutine(TimeToEnemyBulling());
        //             }
        //         } else {
        //             StartCoroutine(TimeToEnemyBulling());
        //         }

        //         break;
        //         // // if already doing bulling
        //         // if (bullingCoroutineStatus != null) return;
        //         // targetPlayer = CheckEnemiesBricks();
        //         // if (targetPlayer == null) _currentBotState = BotState.Idle;
        //         // bullingCoroutineStatus = StartCoroutine(EnemyBulling(targetPlayer));
        //         // if (bullingCoroutineStatus == null) {
        //         //     _currentBotState = BotState.Idle;
        //         // }
        //         // break;
        // }


    }

    // void OnBotLostBrick(GameManager.MyColor color) {
    //     if (bricks.Count < 1) {
    //         _currentBotState = BotState.TakeBrick;
    //         FindNearBrick();
    //     }
    // }

    new void Start() {
        base.Start();
        StartCoroutine(StateMachine());
        StartCoroutine(UpdateDestination());

    }

    private NearPlayer GetNearPlayerWithBricksMoreThatMe() {
        NearPlayer tempPlayer = null;
        var players = GameObject.FindObjectsOfType<BasePlayer>();
        foreach (var player in players) {
            if (player.gameObject == this.gameObject) continue;
            if ((tempPlayer == null && player.bricks.Count > bricks.Count && Vector3.Distance(player.transform.position, transform.position) < 2f) ||
                (player.bricks.Count > bricks.Count && Vector3.Distance(player.transform.position, transform.position) < tempPlayer?.distance)) {
                tempPlayer = new NearPlayer();
                tempPlayer.distance = Vector3.Distance(player.transform.position, transform.position);
                tempPlayer.player = player;
            }
        }
        return tempPlayer;
    }

    class PlayerScriptAndDistancePlusTranform {
        public float distance;

        public Transform transform;
        public BasePlayer playerScript;

        public PlayerScriptAndDistancePlusTranform(BasePlayer playerScript, float distance, Transform transform) {
            this.distance = distance;
            this.transform = transform;
            this.playerScript = playerScript;
        }
    }

    private PlayerScriptAndDistancePlusTranform CheckEnemiesBricks() {
        // get all player's brics, and if bricks less that in bot, and distance not far, go to it.
        var playersBricks = new List<PlayerScriptAndDistancePlusTranform>();
        PlayerScriptAndDistancePlusTranform instance = null;

        foreach (var player in GameManager.Instance.players) {
            if (player == null) continue;
            if (player.gameObject != this.gameObject) {
                var distance = Vector3.Distance(this.transform.position, player.transform.position);
                if (distance < GameManager.Instance.enemyBullingThreshold) {
                    playersBricks.Add((new PlayerScriptAndDistancePlusTranform(player, distance, player.transform)));
                }
            }
        }


        foreach (var item in playersBricks) {
            if ((instance == null && bricks.Count > 2 && item.playerScript.bricks.Count > 0 && bricks.Count > item.playerScript.bricks.Count) ||
            (item.playerScript.bricks.Count > instance?.playerScript.bricks.Count && item.playerScript.bricks.Count < bricks.Count && item.playerScript.bricks.Count > 0)) {
                instance = item;
            }
        }

        if (instance != null) {
            return instance;
            // _currentBotState = BotState.Bulling;
            // StartCoroutine(EnemyBulling((PlayerScriptAndDistancePlusTranform)instance));
        }
        return null;

    }



    IEnumerator EnemyBulling(PlayerScriptAndDistancePlusTranform instance) {
        _currentTarget = instance.transform;
        var timeToBulling = 3f;
        float checkingTime = .2f;
        bool isTrue = true;
        while (isTrue && timeToBulling > 0) {
            yield return new WaitForSeconds(checkingTime);
            timeToBulling -= checkingTime;
            if (instance.playerScript.bricks.Count < bricks.Count && instance.playerScript.bricks.Count > 0 &&
            Vector3.Distance(instance.transform.position, transform.position) <= GameManager.Instance.enemyBullingThreshold) {
                _currentTarget = instance.transform;
            } else {
                isTrue = false;
            }
        }
        _currentBotState = BotState.TakeBrick;
        // FindNearBrick();
        yield return null;
    }

    IEnumerator TimeToEnemyBulling() {
        yield return new WaitForSeconds(3);
        isBulling = false;
    }


    IEnumerator CheckRemainingDistanceForBot() {
        var botPosition = transform.position;
        while (true) {
            // If bot stopped and don't move let's kick him :)))
            if (botPosition == transform.position) {
                if (_currentBotState == BotState.TakeBrick) {
                    _currentBotState = BotState.TakeBrick;
                    FindNearBrick();
                } else {
                    _currentBotState = BotState.TakeLadder;
                    GetBestLadder();
                }
            }

            botPosition = transform.position;
            // Check that we a in a last ladder, and switch agent destination to door
            if (_currentTarget == null && _currentBotState == BotState.TakeBrick) {
                _currentBotState = BotState.TakeLadder;
                GetBestLadder();
            }
            if (_agent.enabled && _currentBotState == BotState.TakeLadder && _agent.remainingDistance < 0.2f) {
                GetBestLadder();
            }
            yield return new WaitForSeconds(.4f);
        }
    }


    IEnumerator UpdateDestination() {
        while (true) {


            if (_botPosition == transform.position) {
                _checkStayBrick = true;
            }

            _botPosition = transform.position;

            if (_agent.enabled && _currentTarget != null) {
                _agent.destination = _currentTarget.position;
            }
            yield return new WaitForSeconds(.4f);
        }
    }


    private void OnTriggerStay(Collider collider) {
        if (_checkStayBrick && (collider.tag == color.ToString() || collider.tag == "Free") && collider.gameObject.layer != LayerMask.NameToLayer("Stairs")) {
            _checkStayBrick = false;
            AddBrickToPlayer(collider.gameObject);
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
            RaycastHit ray;
            if (Physics.Raycast(transform.position, Vector3.down, out ray, 10f)) {
                if (ray.transform.gameObject.layer == LayerMask.NameToLayer("Stairs")) return;
                CheckPlayerCollision(collider);
            }
            // if (bricks.Count < 1) {
            //     FindNearBrick();
            // }

        } else if ((collider.tag == color.ToString() || collider.tag == "Free") && collider.gameObject.layer != LayerMask.NameToLayer("Stairs")) {
            AddBrickToPlayer(collider.gameObject);
            // if (bricks.Count > botBricksThreshold) {
            //     _currentBotState = BotState.TakeLadder;
            //     GetBestLadder();
            // } else {
            //     FindNearBrick(collider.gameObject);
            // }
        } else if (collider.CompareTag("Bonus")) {
            var bonusScript = collider.GetComponent<Bonus>();
            GetBunusEffect(bonusScript.type);
            bonusScript.Destory();
        }
    }

    NearBrick GetNearBrick() {
        NearBrick nBrick = null;
        foreach (var brickScript in GameObject.FindObjectsOfType<Brick>()) {
            if (brickScript.CompareTag(color.ToString()) || brickScript.CompareTag("Free")
                && brickScript.transform.position.y < transform.position.y - 0.5f) {
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


    BestLadder GetBestLadder() {

        var ladders = GameObject.FindObjectsOfType<Ladder>();
        var laddersValues = new List<BestLadder>();

        BestLadder bestLadder = null;
        foreach (var ladder in ladders) {
            var tempNearLadder = new BestLadder(ladder.checkPosition,
                                                Vector3.Distance(ladder.checkPosition.position, transform.position),
                                                ladder.GetCountByColorTag(color),
                                                ladder);
            // Fix it. It cam pick first ladder where already has bricks.
            // TODO:: Fix bots bug when they stop on second floor. It's fixed by set nearLadder to final position!!!
            laddersValues.Add(tempNearLadder);

            // Debug.Log("Ladder position " + tempNearLadder.transform.position.y + " bot position: " + transform.position.y);
            if ((bestLadder == null && tempNearLadder.transform.position.y - 2 > transform.position.y) ||
                (tempNearLadder.colorCount >= bestLadder?.colorCount && tempNearLadder.transform.position.y - 2 > transform.position.y && tempNearLadder.transform.position.y - 2 <= bestLadder?.transform.position.y)) {

                bestLadder = tempNearLadder;
            }
        }

        return bestLadder;
    }

    class NearPlayer {
        public BasePlayer player;
        public float distance;
    }

    class NearBrick {
        public Transform transform;
        public float distance;
    }

    class BestLadder : NearBrick {
        public int colorCount = 0;
        public Ladder ladderScript;

        public BestLadder(Transform transform, float distance, int colorCount, Ladder ladderScript) {
            this.transform = transform;
            this.colorCount = colorCount;
            this.distance = distance;
            this.ladderScript = ladderScript;
        }
    }

}
