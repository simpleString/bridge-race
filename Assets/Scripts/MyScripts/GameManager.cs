using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public int playersCount = 3;

    public float bonusTime = 5f;

    public float brickForce = 1000f;
    public float playersForce = 1000f;


    public enum MyColor {
        blue,
        green,
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

    public static void Vibrate(int timeInMs) {
        if (Store.Store.IsVibrationOn)
            Vibration.Vibrate(timeInMs);
    }

    public static Color GetUnityColorByMyColor(MyColor color) {
        switch (color) {
            case MyColor.blue:
                return new Color(.13f, .36f, .94f);
            case MyColor.green:
                return new Color(.19f, 1f, .15f);
            case MyColor.red:
                return new Color(1f, .15f, .15f);
            case MyColor.brown:
                return new Color(0.64f, .16f, .16f);
            case MyColor.orange:
                return new Color(1, 0.64f, 0);
            case MyColor.purple:
                return new Color(0.5f, 0, 0.5f);
            case MyColor.yellow:
                return new Color(1f, 1, .15f);
            default:
                return Color.black;
        }
    }

    public List<BasePlayer> players = new List<BasePlayer>();

    public MyColor playerColor;
    public Bot botPrefab;

    public static GameManager Instance;

    public bool IsPlayedWithPlayer = true;

    public UI.MainUI managerUI;
    public float enemyBullingThreshold = 2; // collider radius multiplier for player bulling

    public GameMode currentGameMode = GameMode.Game;

    public enum GameMode {
        Game,
        Menu
    }

    void Awake() {

        Vibration.Init();

        var level = FindObjectOfType<LevelManager>();
        if (level != null) {
            currentGameMode = GameMode.Menu;
            FindObjectOfType<Player>().gameObject.SetActive(false);
            FindObjectOfType<UI.MainUI>().gameObject.SetActive(false);
            FindObjectsOfType<Camera>()[1].gameObject.SetActive(false);
            FindObjectsOfType<Light>()[1].gameObject.SetActive(false);
            IsPlayedWithPlayer = false;
            Debug.Log("Hello i'm in menu mode");
        } else {
            playerColor = Store.Store.PlayerColor;
            Time.timeScale = 1;
        }
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
                players.Add(Player.Instance);
                continue;
            }
            if (color == MyColor.black) continue;
            var newBot = Instantiate(botPrefab.gameObject, Vector3.up, Quaternion.identity);
            var newBotScript = newBot.GetComponent<Bot>();
            players.Add(newBotScript);
            newBotScript.color = color;
        }


    }

    public void GameOver() {

    }

    public void StopGame() {
        Time.timeScale = 0f;
    }

    public void ResumeGame() {
        Time.timeScale = 1f;
    }

    public void GameWin(GameObject player) {
        StopGame();
        if (IsPlayedWithPlayer) {
            if (player.GetComponent<BasePlayer>().color == playerColor) {
                var number = System.Int32.Parse(Store.Store.CurrentLevel.Split('_')[1]);
                if (++number <= System.Int32.Parse(Store.Store.MaxLevel.Split('_')[1])) {
                    Store.Store.CurrentLevel = "Level_" + number;
                }
                managerUI.OnWinTrigger();
            } else {
                managerUI.OnWinTrigger();
            }
        } else {
            // TODO:: Reload this scene again
        }
    }



}