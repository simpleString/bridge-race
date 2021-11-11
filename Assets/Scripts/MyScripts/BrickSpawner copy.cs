using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BrickSpawner1 : MonoBehaviour {

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


    int _allCount;

    void Awake() {
        InitBricks();
        if (isActive)
            StartCoroutine(UpdateBricksArray());
    }


    void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Player") {
            isActive = true;
            StartCoroutine(UpdateBricksArray());
        }
    }

    void InitBricks() {

        for (var i = 0; i < GameManager.Instance.players.Count; i++) {
            GameManager.Instance.players[i].playerLostBrick += OnPlayerLostBrick;
            _playerDict.Add(i, 0);
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

    private void OnPlayerLostBrick(GameManager.MyColor color) {
        _playerDict[(int)color]--;
    }

    IEnumerator UpdateBricksArray() {
        while (true) {
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
                        GenerateBricksForPlayers(newBrickObject, x, y);
                        _bricksMap[x, y] = newBrickObject;
                    }
                }
            }
            yield return new WaitForSeconds(2f);
        }
    }

    void GenerateBricksForPlayers(Brick brick, int x, int y) {
        var allBricksComplite = true;
        foreach (var item in _playerDict) {
            if (item.Value != _bricksPerPlayer) allBricksComplite = false;
        }
        if (!allBricksComplite) {
            var isTrue = true;

            while (isTrue) {
                var color = Random.Range(0, GameManager.Instance.playersCount);
                if (_playerDict[color] < _bricksPerPlayer) {
                    isTrue = false;
                    _playerDict[color]++;
                    SetupColorForBrick(brick, color, x, y);
                }
            }
        }
    }

    void SetupColorForBrick(Brick brick, int colorInt, int x, int y) {

        var color = (GameManager.MyColor)colorInt;
        brick.Init(color, x, y);
    }
}
