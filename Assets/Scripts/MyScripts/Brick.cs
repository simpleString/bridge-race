using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Brick : MonoBehaviour {

    Material _material;
    Renderer _renderer;

    public BrickSpawner spawner;

    public int x;
    public int y;

    public bool isPickable = true;

    public System.Action<Brick> onDestroy;

    void Awake() {
        _renderer = GetComponent<Renderer>();
        _material = _renderer.material;
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

    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log(collision.gameObject.layer);
        if (!collision.gameObject.CompareTag("Player") || !collision.gameObject.CompareTag("Untagged"))
            isPickable = true;
    }


    public void Destroy() {
        Destroy(this.gameObject);
    }
}

