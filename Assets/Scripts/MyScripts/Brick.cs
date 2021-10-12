using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{

    Material _material;
    Renderer _renderer;

    public int x;
    public int y;
    
    public System.Action<Brick> onDestroy;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _material = _renderer.material;
    }

    void Update()
    {

    }


    public void Init(Color color, int _x, int _y) {
        x = _x;
        y = _y;
        _material.color = color;
        print(_material.color);
    }

    public void Destroy() {
        onDestroy?.Invoke(this);
    }
}

