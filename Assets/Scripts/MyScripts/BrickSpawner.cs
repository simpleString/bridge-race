using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;


public class BrickSpawner : MonoBehaviour {

    public List<Transform> bonusPlaces; // places where can spawn bonuses

    public Transform brickPrefab; // prefab of brick whitch used by spawner
    public Transform basePool;

    public float xUserOffset; // additional user offset between brick
    public float yUserOffset;

    public bool isActive = false;
    Brick[,] _bricksMap;

    float _lengthX;
    float _lengthY;
    float _lengthZ; // length of basePlatform
    float _offsetX; // length of brickPrefab
    float _offsetY;
    int _xCount;
    int _yCount;

    int _bricksPerPlayer;
    private readonly Dictionary<int, int> _playerDict = new Dictionary<int, int>();

    private readonly List<GameObject> _currentPlayersOnSpawner = new List<GameObject>();


    int _allCount; // All brick count on current spawner



    void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Player" && !_currentPlayersOnSpawner.Contains(collider.gameObject)) {
            var basePlayer = collider.GetComponent<BasePlayer>();
            _playerDict.Add((int)basePlayer.color, 0);
            basePlayer.actionPlayerLostBrick += OnPlayerLostBrick;
            _currentPlayersOnSpawner.Add(collider.gameObject);
            if (isActive) {
                UpdateBricksArray();
            }
        }

        if (!isActive) {
            isActive = true;
            InitBricks();
            UpdateBricksArray();
            StartCoroutine(GenerateBonus());
        }
    }

    void InitBricks() {

        _lengthX = (float)Math.Round(GetComponent<Renderer>().bounds.size.x, 2);
        _lengthY = (float)Math.Round(GetComponent<Renderer>().bounds.size.z, 2);
        _lengthZ = (float)Math.Round(GetComponent<Renderer>().bounds.size.y, 2);


        _offsetY = (float)Math.Round(brickPrefab.GetComponent<Renderer>().bounds.size.z, 2);
        _offsetX = (float)Math.Round(brickPrefab.GetComponent<Renderer>().bounds.size.x, 2);

        _xCount = Mathf.RoundToInt(_lengthX / (_offsetX + xUserOffset));
        _yCount = Mathf.RoundToInt(_lengthY / (_offsetY + yUserOffset));

        _allCount = _xCount * _yCount;

        _bricksMap = new Brick[_xCount, _yCount];

        _bricksPerPlayer = Mathf.CeilToInt(_allCount / (float)GameManager.Instance.playersCount);
    }


    private void OnPlayerLostBrick(GameManager.MyColor color) {
        // TODO:: needs to check if it's active now it's decrease from all spawners
        if (_playerDict[(int)color] > 0) {
            _playerDict[(int)color]--;
            UpdateBricksArray();
        }

    }


    void UpdateBricksArray() {
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
            while (_playerDict[(int)tempPlayerScript.color] < _bricksPerPlayer && nullBrickArray.Count > 0) {
                _playerDict[(int)tempPlayerScript.color]++;

                var randomPosition = nullBrickArray[Random.Range(0, nullBrickArray.Count)];
                nullBrickArray.Remove(randomPosition);

                Transform newBrick = Instantiate(brickPrefab,
                                    new Vector3(
                                        transform.position.x - _lengthX / 2f + randomPosition.x * (_offsetX + xUserOffset),
                                        basePool.position.y + _lengthZ / 3f,
                                        transform.position.z - _lengthY / 2f + randomPosition.y * (_offsetY + yUserOffset)),
                                        Quaternion.identity
                                    );
                newBrick.parent = basePool;
                var newBrickObject = newBrick.GetComponentInChildren<Brick>();
                SetupColorForBrick(newBrickObject, (int)tempPlayerScript.color, randomPosition.x, randomPosition.y);
                _bricksMap[randomPosition.x, randomPosition.y] = newBrickObject;
            }
        }


    }
    void SetupColorForBrick(Brick brick, int colorInt, int x, int y) {

        var color = (GameManager.MyColor)colorInt;
        brick.Init(color, x, y);
    }

    // Generate bonus. Now it's generate only one bonus per platform
    IEnumerator GenerateBonus() {
        bool isGiveBonus = false;
        while (!isGiveBonus) {
            if (_currentPlayersOnSpawner.Count > 2) {
                // get place for bonus
                var place = bonusPlaces[Random.Range(0, bonusPlaces.Count)];
                // get random bonus
                if (place == null) isGiveBonus = true; // If platform without places don't generate bonuses for it
                var bonus = GameManager.Instance.bonuses[Random.Range(0, GameManager.Instance.bonuses.Count)];
                // var bonus = GameManager.Instance.bonuses[Random.Range(1, 1)];
                isGiveBonus = true;
                Debug.Log("imfdskjlfsdjk");
                var newBonus = Instantiate(bonus, place);
            }
            yield return new WaitForSeconds(1f);
        }

    }
}
