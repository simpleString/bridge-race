using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BrickSpawner : MonoBehaviour
{

    [System.Serializable]
    public struct BrickPrefab {
        public string name;
        public Transform brickPrefab;
    }

    public Transform spawnPool;
    public List<BrickPrefab> bricksPrefabs = new List<BrickPrefab>();
    public Transform basePool;

    private GameManager _gameManager;

    public float xUserOffset;
    public float yUserOffset;
    Brick[,] _bricksMap;

    float _lengthX;
    float _lengthY;
    float _lengthZ;
    float _offsetZ;
    float _offsetX;
    float _offsetY;
    int _xCount;
    int _yCount;

    int _bricksPerPlayer;

    Dictionary<int,int> _playerDict = new Dictionary<int,int>() {
        {0, 0},
        {1, 0},
        {2, 0},
    };

    public int playersCount = 3;


    int _allCount;

    void Awake() {
        InitBricks();
        // SpawnBricks();
        StartCoroutine(UpdateBricksArray());
    }

    void Start() {
        _gameManager = GameManager.Instance;
    }

    void InitBricks() {
        _lengthX = (float) Math.Round(spawnPool.GetComponent<Renderer>().bounds.size.x, 2);
        _lengthY = (float) Math.Round(spawnPool.GetComponent<Renderer>().bounds.size.z, 2);
        _lengthZ = (float) Math.Round(spawnPool.GetComponent<Renderer>().bounds.size.y, 2);
        

        _offsetX = (float) Math.Round(bricksPrefabs[0].brickPrefab.GetComponent<Renderer>().bounds.size.x, 2);
        _offsetY = (float) Math.Round(bricksPrefabs[0].brickPrefab.GetComponent<Renderer>().bounds.size.z, 2);
        _offsetZ = (float) Math.Round(bricksPrefabs[0].brickPrefab.GetComponent<Renderer>().bounds.size.y, 2);

        _xCount = Mathf.RoundToInt(_lengthX / (_offsetX + xUserOffset));
        _yCount = Mathf.RoundToInt(_lengthY / (_offsetY + yUserOffset));

        _allCount = _xCount * _yCount;

        _bricksMap = new Brick[_xCount, _yCount];

        _bricksPerPlayer = Mathf.CeilToInt((float) _allCount / playersCount);
    }

    IEnumerator UpdateBricksArray() {
        while(true) {
            for (var y = 0; y < _yCount; y++) {
                for (var x = 0; x < _xCount; x++) {
                    if (_bricksMap[x, y] == null || _bricksMap[x, y].isDead) {
                        Transform newBrick;
                        newBrick = Instantiate(bricksPrefabs[0].brickPrefab,
                        new Vector3(
                            spawnPool.localPosition.x - _lengthX / 2f + x * (_offsetX + xUserOffset),
                            basePool.position.y + _lengthZ / 3f,
                            spawnPool.localPosition.z - _lengthY / 2f + y * (_offsetY + yUserOffset)), 
                            Quaternion.identity 
                    );
                        newBrick.parent = basePool;
                        var newBrickObject = newBrick.GetComponentInChildren<Brick>();
                        GenerateRandomBrick(newBrickObject, x, y);
                        _bricksMap[x, y] = newBrickObject;
                    }
                }
            }
            yield return new WaitForSeconds(2f);
        }
    }

    // void OnBrickDestroy(Brick brick){
    //     for (var y = 0; y < _yCount; y++) {
    //         for (var x = 0; x < _xCount; x++) {
    //             if (x == brick.x && y == brick.y) {
    //                 _bricksMap[x, y] = null;
    //             }
    //         }
    //     }
    // }

    // void SpawnBricks() {
    //     for (var y = 0; y < _yCount; y++) {
    //         for (var x = 0; x < _xCount; x++) {
    //             Transform newBrick;
    //             newBrick = Instantiate(bricksPrefabs[0].brickPrefab,
    //             new Vector3(
    //                 spawnPool.localPosition.x - _lengthX / 2f + x * (_offsetX + xUserOffset),
    //                 basePool.position.y + _lengthZ / 3f, // TODO:: I don't know how it work Fix it!!!
    //                 spawnPool.localPosition.z - _lengthY / 2f + y * (_offsetY + yUserOffset)), 
    //                 Quaternion.identity
    //         );
    //             newBrick.parent = basePool;
    //             var newBrickObject = newBrick.GetComponentInChildren<Brick>();
    //             newBrickObject.onDestroy += OnBrickDestroy;
    //             GenerateBricksForPlayers(newBrickObject, x, y);
    //             _bricksMap[x, y] = newBrick;
    //         }
    //     }
    // }


    void GenerateRandomBrick(Brick brick, int x, int y) {
        var color = Random.Range(0, playersCount);
        SetupColorForBrick(brick, color, x, y);
    }
 
    void GenerateBricksForPlayers(Brick brick, int x, int y) {
            var isTrue = true;
            do {
                var color = Random.Range(0, playersCount);
                if (_playerDict[color] < _bricksPerPlayer) {
                    isTrue = false;
                    _playerDict[color]++;
                    SetupColorForBrick(brick, color, x, y);
                }
            } while (isTrue);
    }

    void SetupColorForBrick(Brick brick, int colorInt, int x, int y) {

        var color = (GameManager.MyColor) colorInt;
        brick.Init(color, x, y);
    }
}
