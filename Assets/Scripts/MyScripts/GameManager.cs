using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public int playersCount = 3;

    public enum MyColor {
        green,
        blue,
        red,
        black
    }

    public static Color GetUnityColorByMyColor(MyColor color) {
        switch (color) {
            case MyColor.blue:
                return Color.blue;
            case MyColor.green:
                return Color.green;
            case MyColor.red:
                return Color.red;
            default:
                return Color.black;
        }
    }

    public static MyColor GetMyColorByUnityColor(Color color) {
        if (color == Color.blue) return MyColor.blue;
        else if (color == Color.green) return MyColor.green;
        else if (color == Color.red) return MyColor.red;
        else return MyColor.black;
    }

    public List<BasePlayer> players = new List<BasePlayer>();

    public MyColor playerColor = MyColor.blue;  // TODO:: Need to set by user!!!

    public float xBrickOffset;
    public float yBrickOffset;
    public float playersCollisionForce;
    public float jumpTime;

    public Transform basePlatform;

    public Bot botPrefab;

    public static GameManager Instance;

    void Awake() {
        if (Instance != null) {
            DestroyImmediate(this);
        }
        Instance = this;
        InitGame();
    }

    void InitGame() {

        foreach (MyColor color in System.Enum.GetValues(typeof(MyColor))) {
            if (color == playerColor) {
                Player.Instance.Init(color, basePlatform);
                players.Add(Player.Instance);
                continue;
            }
            if (color == MyColor.black) continue;
            var newBot = Instantiate(botPrefab.gameObject, Vector3.up, Quaternion.identity);
            var newBotScript = newBot.GetComponent<Bot>();
            players.Add(newBotScript);
            newBotScript.Init(color, basePlatform);
        }

        foreach (var item in GameObject.FindObjectsOfType<BrickSpawner>()) {
            item.StartSpawn();
        }



    }

    public void GameOver() {

    }

    public void GameWin() {

    }
}