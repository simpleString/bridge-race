using UnityEngine;


public class Stair : MonoBehaviour {
    public void ChangeColor(GameManager.MyColor color) {
        var renderer = GetComponent<Renderer>();
        this.tag = GameManager.Instance.playerColor.ToString(); // FIXME:: Needs optimize to work with bots!!!
        renderer.material.color = GameManager.GetUnityColorByMyColor(color);
        renderer.enabled = true;
    }
}