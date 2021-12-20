using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FollowCamera : MonoBehaviour {
    [SerializeField] private float SmoothTime = 0.3f;
    [SerializeField] private Transform target;
    private Vector3 offset = new Vector3(0, 2, -5);
    private Vector3 velocity = Vector3.zero;
    private Renderer _texture;
    private Camera _camera;

    private List<Transform> _renderers = new List<Transform>(); // Renderers which change opacity

    void Awake() {
        _camera = GetComponent<Camera>();
    }

    private void Start() {
        offset = transform.position - target.position;
        Vector3 targetPosition = target.position + offset;
        transform.position = targetPosition;
    }


    void Update() {



        // Ray ray = new Ray(transform.position, transform.forward);
        // var dir = Player.Instance.transform.position + Vector3.up * 2 - transform.position;
        // var hits = Physics.RaycastAll(transform.position, dir, 10000f);
        // Debug.DrawRay(transform.position, dir, Color.yellow, 100000f);
        // Renderer tempRenderer;
        // List<Transform> deleteItems = new List<Transform>(_renderers);
        // for (int i = 0; i < hits.Length; i++) {
        //     Debug.Log(hits[i].collider.tag);
        //     if (hits[i].collider.CompareTag("Player")) {
        //         for (int j = i; j < hits.Length; j++) {
        //             if (hits[i].collider.CompareTag("platform")) {
        //                 deleteItems.Remove(hits[i].transform);
        //             }
        //         }
        //         break;
        //     } else if (hits[i].collider.CompareTag("platform")) {
        //         tempRenderer = hits[i].transform.GetComponent<Renderer>();
        //         if (!_renderers.Contains(hits[i].transform)) {
        //             _renderers.Add(hits[i].transform);
        //         } else {
        //             deleteItems.Remove(hits[i].transform);
        //         }
        //         // foreach (var material in tempRenderer.materials) {
        //         //     var color = material.color;
        //         //     color.a = .5f;
        //         // }

        //         var texture = hits[i].collider.GetComponent<Renderer>();
        //         texture.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        //     }
        // }


        // foreach (var item in deleteItems) {
        //     if (item.gameObject.layer != LayerMask.NameToLayer("Spawner"))
        //         item.transform.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        //     // _renderers.Remove(item);
        // }

        // RaycastHit raycastHit;
        // if (Physics.Raycast(transform.position, transform.forward, out raycastHit, 100000f)) {
        //     Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * raycastHit.distance, Color.yellow);
        //     Debug.Log(raycastHit.collider.tag);
        //     if (raycastHit.collider.CompareTag("platform")) {
        //         _texture = raycastHit.collider.GetComponent<Renderer>();
        //         _texture.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        //     } else if (_texture != null) {
        //         _texture.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        //         _texture = null;
        //     }
        // }
    }

    public void TriggerEnter(LayerMask layer) {
        if (layer == 0) return;
        _camera.cullingMask += layer;
    }

    private void LateUpdate() {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);
        transform.LookAt(target);
    }
}
