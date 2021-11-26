using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour {

    Material _material;
    Renderer _renderer;

    public int x;
    public int y;

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

    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("platform"))
            isPickable = true;
    }


    public void Destroy() {
        Destroy(this.gameObject);
    }
}

