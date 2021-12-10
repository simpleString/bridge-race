using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using static GameManager;

public class Brick : MonoBehaviour {

    Material _material;
    Renderer _renderer;
    Collider _collider;

    public BrickSpawner spawner;

    public int x;
    public int y;

    public bool isPickable = true;

    public System.Action<Brick> onDestroy;

    void Awake() {
        _collider = GetComponent<Collider>();
        _renderer = GetComponent<Renderer>();
        _material = _renderer.materials[2];
    }

    public void InitPortableBrick(MyColor color) {
        _material.color = GameManager.GetUnityColorByMyColor(color);
        tag = MyConstants.TagNull;
    }

    public void Init(GameManager.MyColor color, int _x, int _y, BrickSpawner _spawner) {
        tag = color.ToString();
        x = _x;
        y = _y;
        _material.color = GameManager.GetUnityColorByMyColor(color);
        spawner = _spawner;
    }

    public void InitAfterDrop(GameManager.MyColor color) {
        tag = color.ToString();
        _material.DOColor(GameManager.GetUnityColorByMyColor(color), 1f);
        // _material.color = GameManager.GetUnityColorByMyColor(color);
        isPickable = false;
        StartCoroutine(StartDeathTimer());
    }

    IEnumerator StartDeathTimer() {
        yield return new WaitForSeconds(3f);
        Destroy(this.gameObject);
        foreach (var item in GameObject.FindObjectsOfType<BrickSpawner>()) {
            Debug.Log("Randomize bricks on platforms");
            item.UpdateOneRandomBrick();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log(collision.gameObject.layer);
        if (collision.gameObject.CompareTag("platform"))
            // if (!collision.gameObject.CompareTag("Player") || !collision.gameObject.CompareTag("Untagged"))
            isPickable = true;
        // if (collision.gameObject.CompareTag("Player")) {
        //     Physics.IgnoreCollision(collision.collider, _collider);
        // }
    }


    public void Destroy() {
        StopAllCoroutines();
        Destroy(this.gameObject);
    }
}

