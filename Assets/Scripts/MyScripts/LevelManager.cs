using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    void Awake() {
        Debug.Log("CUrrent level is: " + Store.Store.CurrentLevel);
        SceneManager.LoadScene(Store.Store.CurrentLevel, LoadSceneMode.Additive);

    }
    void Start() {
        FindObjectOfType<GameManager>().currentGameMode = GameManager.GameMode.Menu;
    }
}
