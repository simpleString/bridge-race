using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    void Awake() {
        Debug.Log("CUrrent level is: " + Store.Store.CurrentLevel);
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Store.Store.CurrentLevel, LoadSceneMode.Additive);
        while (!asyncLoad.isDone) yield return null;
        var scene = SceneManager.GetSceneByName(Store.Store.CurrentLevel);
        SceneManager.SetActiveScene(scene);
    }

    IEnumerator UnloadSceneAndLoadAnother(string unload) {
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(unload);
        while (!asyncLoad.isDone) yield return null;
        yield return LoadScene();
    }

    public void SetLevel(string level) {
        Store.Store.CurrentLevel = level;
        if (level == "Level_1") {
            // StartCoroutine(UnloadSceneAndLoadAnother("Level_2"));
            // SceneManager.UnloadSceneAsync("Level_2");
        } else {
            // StartCoroutine(UnloadSceneAndLoadAnother("Level_1"));
            // SceneManager.UnloadSceneAsync("Level_1");
        }

        SceneManager.LoadSceneAsync(0);
        // SceneManager.LoadSceneAsync(Store.Store.CurrentLevel, LoadSceneMode.Additive);
    }

    void Start() {
        // FindObjectOfType<GameManager>().currentGameMode = GameManager.GameMode.Menu;
    }
}
