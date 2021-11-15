using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour {

    Material _material;
    Renderer _renderer;

    public int x;
    public int y;

    public bool isDead = false;

    public bool isPickable = true;

    public System.Action<Brick> onDestroy;

    void Awake() {
        _renderer = GetComponent<Renderer>();
        _material = _renderer.material;
    }

    public void Init(GameManager.MyColor color, int _x, int _y) {
        this.tag = color.ToString();
        x = _x;
        y = _y;
        _material.color = GameManager.GetUnityColorByMyColor(color);
    }

    public void InitAfterDrop(GameManager.MyColor color) {
        tag = color.ToString();
        _material.color = GameManager.GetUnityColorByMyColor(color);
        isPickable = false;
        StartCoroutine(BrickPickTimer());

    }

    private IEnumerator BrickPickTimer() {
        yield return new WaitForSeconds(3f);
        isPickable = true;
    }

    IEnumerator DeadTimer() {
        yield return new WaitForSeconds(2f);
        isDead = true;

    }


    public void Destroy() {
        // StartCoroutine(DeadTimer());
        Destroy(this.gameObject);
        // onDestroy?.Invoke(this);
    }
}

