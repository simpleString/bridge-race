using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public class BrickSpawner : MonoBehaviour {

    [System.Serializable]
    public struct BrickPrefab {
        public string name;
        public Transform brickPrefab;
    }

    public Transform spawnPool;
    public List<BrickPrefab> bricksPrefabs = new List<BrickPrefab>();
    public Transform basePool;

    public float xUserOffset;
    public float yUserOffset;

    public bool isActive = false;
    Brick[,] _bricksMap;

    float _lengthX;
    float _lengthY;
    float _lengthZ;
    float _offsetX;
    float _offsetY;
    int _xCount;
    int _yCount;

    int _bricksPerPlayer;
    readonly Dictionary<int, int> _playerDict = new Dictionary<int, int>();

    private List<GameObject> _currentPlayersOnSpawner = new List<GameObject>();


    int _allCount;

    // void Awake() {

    // }


    void OnTriggerEnter(Collider collider) {

        if (collider.tag == "Player" && !_currentPlayersOnSpawner.Contains(collider.gameObject))
            _currentPlayersOnSpawner.Add(collider.gameObject);

        if (!isActive) {
            isActive = true;
            StartCoroutine(UpdateBricksArray());
        }
    }

    void InitBricks() {

        Debug.Log(GameManager.Instance.players.Count);
        for (var i = 0; i < GameManager.Instance.players.Count; i++) {
            GameManager.Instance.players[i].playerLostBrick += OnPlayerLostBrick;
            _playerDict.Add((int)GameManager.Instance.players[i].myColor, 0);
        }

        _lengthX = (float)Math.Round(spawnPool.GetComponent<Renderer>().bounds.size.x, 2);
        _lengthY = (float)Math.Round(spawnPool.GetComponent<Renderer>().bounds.size.z, 2);
        _lengthZ = (float)Math.Round(spawnPool.GetComponent<Renderer>().bounds.size.y, 2);


        _offsetX = (float)Math.Round(bricksPrefabs[0].brickPrefab.GetComponent<Renderer>().bounds.size.x, 2);
        _offsetY = (float)Math.Round(bricksPrefabs[0].brickPrefab.GetComponent<Renderer>().bounds.size.z, 2);

        _xCount = Mathf.RoundToInt(_lengthX / (_offsetX + xUserOffset));
        _yCount = Mathf.RoundToInt(_lengthY / (_offsetY + yUserOffset));

        _allCount = _xCount * _yCount;

        _bricksMap = new Brick[_xCount, _yCount];

        _bricksPerPlayer = Mathf.CeilToInt(_allCount / (float)GameManager.Instance.playersCount);
    }

    public void StartSpawn() {
        InitBricks();
        if (isActive) {
            StartCoroutine(UpdateBricksArray());
        }
    }

    private void OnPlayerLostBrick(GameManager.MyColor color) {
        // TODO:: needs to check if it's active now it's decrease from all spawners
        if (_playerDict[(int)color] > 0) {
            Debug.Log("_playerDict: " + _playerDict[(int)color] + " _brickPerPlayer: " + _bricksPerPlayer);
            _playerDict[(int)color]--;
        }

    }


    IEnumerator UpdateBricksArray() {
        while (true) {
            // get array of null bricks on map
            var nullBrickArray = new List<Vector2Int>();
            for (var y = 0; y < _yCount; y++) {
                for (var x = 0; x < _xCount; x++) {
                    if (_bricksMap[x, y] == null)
                        nullBrickArray.Add(new Vector2Int(x, y));
                }
            }

            foreach (var item in _currentPlayersOnSpawner) {
                var tempPlayerScript = item.GetComponent<BasePlayer>();
                while (_playerDict[(int)tempPlayerScript.myColor] < _bricksPerPlayer) {
                    Debug.Log("_playerDict: " + _playerDict[(int)tempPlayerScript.myColor] + " _brickPerPlayer: " + _bricksPerPlayer);
                    Debug.Log("_currentPlayersOnSpawner: " + _currentPlayersOnSpawner.Count);

                    _playerDict[(int)tempPlayerScript.myColor]++;

                    var randomPosition = nullBrickArray[Random.Range(0, nullBrickArray.Count)];
                    nullBrickArray.Remove(randomPosition);

                    Transform newBrick = Instantiate(bricksPrefabs[0].brickPrefab,
                                        new Vector3(
                                            spawnPool.localPosition.x - _lengthX / 2f + randomPosition.x * (_offsetX + xUserOffset),
                                            basePool.position.y + _lengthZ / 3f,
                                            spawnPool.localPosition.z - _lengthY / 2f + randomPosition.y * (_offsetY + yUserOffset)),
                                            Quaternion.identity
                                        );
                    newBrick.parent = basePool;
                    var newBrickObject = newBrick.GetComponentInChildren<Brick>();
                    SetupColorForBrick(newBrickObject, (int)tempPlayerScript.myColor, randomPosition.x, randomPosition.y);
                    _bricksMap[randomPosition.x, randomPosition.y] = newBrickObject;
                }
            }

            // for (var y = 0; y < _yCount; y++) {
            //     for (var x = 0; x < _xCount; x++) {
            //         if (_bricksMap[x, y] == null) {
            //             Transform newBrick;
            //             newBrick = Instantiate(bricksPrefabs[0].brickPrefab,
            //             new Vector3(
            //                 spawnPool.localPosition.x - _lengthX / 2f + x * (_offsetX + xUserOffset),
            //                 basePool.position.y + _lengthZ / 3f,
            //                 spawnPool.localPosition.z - _lengthY / 2f + y * (_offsetY + yUserOffset)),
            //                 Quaternion.identity
            //         );
            //             newBrick.parent = basePool;
            //             var newBrickObject = newBrick.GetComponentInChildren<Brick>();
            //             GenerateBricksForPlayers(newBrickObject, x, y);
            //             _bricksMap[x, y] = newBrickObject;
            //         }
            //     }
            // }



            // _initPlayerDict = new Dictionary<int, int>(_playerDict);
            // while (true) {

            // var isEqual = true;
            // for (int i = 0; i < _initPlayerDict.Count; i++) {

            // }
            // yield return new WaitForSeconds(2f);
            yield return null;
        }
    }

    // IEnumerator UpdateBricksArray() {
    //     while (true) {
    //         for (var y = 0; y < _yCount; y++) {
    //             for (var x = 0; x < _xCount; x++) {
    //                 if (_bricksMap[x, y] == null) {
    //                     Transform newBrick;
    //                     newBrick = Instantiate(bricksPrefabs[0].brickPrefab,
    //                     new Vector3(
    //                         spawnPool.localPosition.x - _lengthX / 2f + x * (_offsetX + xUserOffset),
    //                         basePool.position.y + _lengthZ / 3f,
    //                         spawnPool.localPosition.z - _lengthY / 2f + y * (_offsetY + yUserOffset)),
    //                         Quaternion.identity
    //                 );
    //                     newBrick.parent = basePool;
    //                     var newBrickObject = newBrick.GetComponentInChildren<Brick>();
    //                     GenerateBricksForPlayers(newBrickObject, x, y);
    //                     _bricksMap[x, y] = newBrickObject;
    //                 }
    //             }
    //         }
    //         
    //     }
    // }

    // void GenerateBricksForPlayers(Brick brick, int x, int y) {
    //     var allBricksComplite = true;
    //     foreach (var item in _playerDict) {
    //         if (item.Value != _bricksPerPlayer) allBricksComplite = false;
    //     }
    //     if (!allBricksComplite) {
    //         var isTrue = true;

    //         while (isTrue) {
    //             var color = Random.Range(0, GameManager.Instance.playersCount);
    //             if (_playerDict[color] < _bricksPerPlayer) {
    //                 isTrue = false;
    //                 _playerDict[color]++;
    //                 SetupColorForBrick(brick, color, x, y);
    //             }
    //         }
    //     }
    // }

    void SetupColorForBrick(Brick brick, int colorInt, int x, int y) {

        var color = (GameManager.MyColor)colorInt;
        brick.Init(color, x, y);
    }
}
