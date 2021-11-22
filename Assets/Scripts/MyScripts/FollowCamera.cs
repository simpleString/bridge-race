using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -5);

    private Renderer _texture;
    void Update() {
        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position, transform.forward, out raycastHit, 100000f)) {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * raycastHit.distance, Color.yellow);
            Debug.Log(raycastHit.collider.tag);
            if (raycastHit.collider.tag != "Player") {
                _texture = raycastHit.collider.GetComponent<Renderer>();
                _texture.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            } else if (_texture != null) {
                _texture.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                _texture = null;
            }
        }
        transform.position = target.position + offset;
    }
}
