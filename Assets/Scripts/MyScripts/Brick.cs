using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{

    Material _material;
    Renderer _renderer;

    public int x;
    public int y;

    public bool isDead = false;
    
    public System.Action<Brick> onDestroy;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _material = _renderer.material;
    }

    void Update()
    {

    }


    public void Init(GameManager.MyColor color, int _x, int _y) {
        this.tag = color.ToString();
        x = _x;
        y = _y;
        _material.color = GameManager.GetUnityColorByMyColor(color);
    }

    IEnumerator DeadTimer() {
        yield return new WaitForSeconds(2f);
        isDead = true;
    }

    public void Destroy() {
        StartCoroutine(DeadTimer());
        // onDestroy?.Invoke(this);
    }
}

