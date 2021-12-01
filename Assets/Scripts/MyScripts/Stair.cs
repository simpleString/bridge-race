using UnityEngine;
using DG.Tweening;


public class Stair : MonoBehaviour {
    public void ChangeColor(GameManager.MyColor color) {
        // transform.DOMove(new Vector3(0, 0, 0), 1f).From();

        var renderer = GetComponent<Renderer>();
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        renderer.material.DOColor(GameManager.GetUnityColorByMyColor(color), 1f);
        this.tag = color.ToString();
        // renderer.material.color = GameManager.GetUnityColorByMyColor(color);
        renderer.enabled = true;
    }
}