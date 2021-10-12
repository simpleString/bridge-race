using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -5);
    void Update()
    {
        transform.position = target.position + offset;//Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime);
    }
}
