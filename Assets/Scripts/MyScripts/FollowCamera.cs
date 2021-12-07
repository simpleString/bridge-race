using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {
    [SerializeField] private float SmoothTime = 0.3f;
    [SerializeField] private Transform target;
    private Vector3 offset = new Vector3(0, 2, -5);
    private Vector3 velocity = Vector3.zero;
    private Renderer _texture;

    private void Start() {
        offset = transform.position - target.position;
        Vector3 targetPosition = target.position + offset;
        transform.position = targetPosition;
    }

    void Update() {
        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position, transform.forward, out raycastHit, 100000f)) {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * raycastHit.distance, Color.yellow);
            Debug.Log(raycastHit.collider.tag);
            if (raycastHit.collider.CompareTag("platform")) {
                _texture = raycastHit.collider.GetComponent<Renderer>();
                _texture.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            } else if (_texture != null) {
                _texture.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                _texture = null;
            }
        }
    }

    private void LateUpdate() {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);
        transform.LookAt(target);
    }
}
