using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{

    enum BotDifficult {
        Easy,
        Medium,
        Hard
    }

    public enum Colors {
        Red,
        Geen,
        Blue,
        Black
    }

    public Transform spawnPool;
    public Transform brickPrefab;
    public Transform basePool;

    public float xUserOffset;
    public float yUserOffset;
    Transform[,] _bricksMap;

    float _lengthX;
    float _lengthY;
    float _offsetX;
    float _offsetY;
    int _xCount;
    int _yCount;

    int _bricksPerPlayer;

    Dictionary<int,int> _playerDict = new Dictionary<int,int>() {
        {0, 0},
        {1, 0},
        {2, 0},
        {3, 0}
    };

    Dictionary<int,int> _playerRemovedDict = new Dictionary<int,int>() {
        {0, 0},
        {1, 0},
        {2, 0},
        {3, 0}
    };

    public int playersCount = 3;

    int _countOfDestroyedBricks = 0;

    int _allCount;

    void Awake() {
        InitBricks();
        _bricksPerPlayer = (int)(_allCount / playersCount);
        SpawnBricks();
        StartCoroutine(UpdateBricksArray());
    }


    void InitBricks() {
        _lengthX = spawnPool.localScale.x;
        _lengthY = spawnPool.localScale.z;

        _offsetX = brickPrefab.localScale.x;
        _offsetY = brickPrefab.localScale.z;

        _xCount = Mathf.RoundToInt(_lengthX / (brickPrefab.localScale.x + _offsetX + xUserOffset) + 1); // TODO:: Fix it shit!!!
        _yCount = Mathf.RoundToInt(_lengthY / (brickPrefab.localScale.z + _offsetY + yUserOffset));

        _allCount = _xCount * _yCount;

        _bricksMap = new Transform[_xCount, _yCount];
    }

    IEnumerator UpdateBricksArray() {
        while(true) {
            Debug.Log("hello");
            for (var y = 0; y < _yCount; y++) {
                for (var x = 0; x < _xCount; x++) {
                    if (_bricksMap[x, y] == null) {
                        Transform newBrick;
                        newBrick = Instantiate(brickPrefab,
                        new Vector3(
                            spawnPool.localPosition.x - spawnPool.localScale.x / 2f + x * (brickPrefab.localScale.x + _offsetX + xUserOffset), //+ x * brickPrefab.localScale.x + bricksOffset
                            basePool.position.y + brickPrefab.localScale.y,
                            spawnPool.localPosition.z - spawnPool.localScale.z / 2f + y * (brickPrefab.localScale.z + _offsetY + yUserOffset)), Quaternion.identity // + y * brickPrefab.localScale.z + bricksOffset
                    );
                        newBrick.parent = spawnPool;
                        var newBrickObject = newBrick.GetComponentInChildren<Brick>();
                        newBrickObject.onDestroy += OnBrickDestroy;
                        GenerateRandomBrick(newBrickObject, x, y);
                        _bricksMap[x, y] = newBrick;
                    }
                }
            }
            yield return new WaitForSeconds(2);
        }
    }

    void OnBrickDestroy(Brick brick){
        for (var y = 0; y < _yCount; y++) {
            for (var x = 0; x < _xCount; x++) {
                if (x == brick.x && y == brick.y) {
                    Debug.Log("hello from destroyer");
                    _bricksMap[x, y] = null;
                }
            }
        }
    }

    void SpawnBricks() {
        for (var y = 0; y < _yCount; y++) {
            for (var x = 0; x < _xCount; x++) {
                Transform newBrick;
                newBrick = Instantiate(brickPrefab,
                new Vector3(
                    spawnPool.localPosition.x - spawnPool.localScale.x / 2f + x * (brickPrefab.localScale.x + _offsetX + xUserOffset), //+ x * brickPrefab.localScale.x + bricksOffset
                    basePool.position.y + brickPrefab.localScale.y,
                    spawnPool.localPosition.z - spawnPool.localScale.z / 2f + y * (brickPrefab.localScale.z + _offsetY + yUserOffset)), Quaternion.identity // + y * brickPrefab.localScale.z + bricksOffset
            );
                newBrick.parent = spawnPool;
                var newBrickObject = newBrick.GetComponentInChildren<Brick>();
                newBrickObject.onDestroy += OnBrickDestroy;
                GenerateBricksForPlayers(newBrickObject, x, y);
                _bricksMap[x, y] = newBrick;
            }
        }
    }


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

    void SetupColorForBrick(Brick brick, int color, int x, int y) {
        Color setColor;
        switch (color)
        {
            case 0:
                setColor = Color.red;
                brick.tag = "red";
                print("red");
                break;
            case 1:
                setColor = Color.green;
                brick.tag = "green";
                print("green");
                break;
            case 2:
                setColor = Color.blue;
                brick.tag = "blue";
                print("blue");
                break;
            default:
                setColor = Color.black;
                brick.tag = "black";
                break;
        }
        brick.Init(setColor, x, y);
    }

}
