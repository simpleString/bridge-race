using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{

    Material _material;
    
    public System.Action onDestroy;

    void Awake()
    {
        _material = GetComponent<Renderer>().material;
        
    }

    void Update()
    {
        
    }


    public void SetColor(Color color) {
        _material.color = color;
    }

    public void Destroy() {
        onDestroy?.Invoke();
    }
}

