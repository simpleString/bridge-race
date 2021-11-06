using UnityEngine;


public class Stair : MonoBehaviour {
    public void ChangeColor(GameManager.MyColor color) {
        var renderer = GetComponent<Renderer>();
        this.tag = color.ToString();
        renderer.material.color = GameManager.GetUnityColorByMyColor(color);
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        renderer.enabled = true;
    }
}