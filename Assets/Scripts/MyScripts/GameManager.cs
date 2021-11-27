using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public int playersCount = 3;

    public float bonusTime = 5f;


    public enum MyColor {
        green,
        blue,
        red,
        orange,
        purple,
        yellow,
        brown,
        black
    }

    public enum BonusType {
        Magnet,
        Freeze,
        Double,
        Speed,

    }

    public List<Transform> bonuses;

    public static void Vibrate() {
        if (isVidrationOn)
            Handheld.Vibrate();
    }

    public static Color GetUnityColorByMyColor(MyColor color) {
        switch (color) {
            case MyColor.blue:
                return Color.blue;
            case MyColor.green:
                return Color.green;
            case MyColor.red:
                return Color.red;
            case MyColor.brown:
                return new Color(0.64f, .16f, .16f); //TODO:: check it
            case MyColor.orange:
                return new Color(1, 0.64f, 0);
            case MyColor.purple:
                return new Color(0.5f, 0, 0.5f);
            case MyColor.yellow:
                return Color.yellow;
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

    public Transform basePlatform;

    public Bot botPrefab;

    public static GameManager Instance;

    public bool IsPlayedWithPlayer = true;

    public UI.MainUI managerUI;
    public float enemyBullingThreshold = 2; // collider radius multiplier for player bulling
    private static bool isVidrationOn;

    void Awake() {
        Time.timeScale = 1;
        if (Instance != null) {
            DestroyImmediate(this);
        }
        Instance = this;
        InitGame();
    }

    void InitGame() {
        for (int i = 0; i < playersCount; i++) {
            var color = (MyColor)i;
            if (IsPlayedWithPlayer && color == playerColor) {
                // Player.Instance.Init(color, basePlatform);
                players.Add(Player.Instance);
                continue;
            }
            if (color == MyColor.black) continue;
            var newBot = Instantiate(botPrefab.gameObject, Vector3.up, Quaternion.identity);
            var newBotScript = newBot.GetComponent<Bot>();
            players.Add(newBotScript);
            // newBotScript.Init(color, basePlatform);
            newBotScript.color = color;
        }
        // foreach (MyColor color in System.Enum.GetValues(typeof(MyColor))) {
        //     if (IsPlayedWithPlayer && color == playerColor) {
        //         // Player.Instance.Init(color, basePlatform);
        //         players.Add(Player.Instance);
        //         continue;
        //     }
        //     if (color == MyColor.black) continue;
        //     var newBot = Instantiate(botPrefab.gameObject, Vector3.up, Quaternion.identity);
        //     var newBotScript = newBot.GetComponent<Bot>();
        //     players.Add(newBotScript);
        //     // newBotScript.Init(color, basePlatform);
        //     newBotScript.color = color;
        // }

    }

    public void GameOver() {

    }

    public void GameWin(GameObject player) {
        if (IsPlayedWithPlayer) {
            if (player.GetComponent<BasePlayer>().color == playerColor) {
                managerUI.OnWinTrigger();
            } else {
                managerUI.OnLoseTrigger();
            }
        } else {
            // TODO:: Reload this scene again
        }
    }



}