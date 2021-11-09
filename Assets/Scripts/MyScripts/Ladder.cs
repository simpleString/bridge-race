using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    private List<Transform> _stairs = new List<Transform>();
    public List<Transform> stairsBlock = new List<Transform>();

    public Transform checkPosition;

    void Start()
    {
        foreach (Transform stairBlock in stairsBlock)
        {

            foreach (Transform child in stairBlock)
            {
                _stairs.Add(child);
            }
        }
    }

    public int GetCountByColorTag(GameManager.MyColor color)
    {
        Debug.Log("ColorName: " + color);
        var count = 0;
        foreach (Transform stair in _stairs)
        {
            Debug.Log("String with tag: " + stair.tag);
            if (stair.tag == color.ToString())
            {
                count++;
            }
        }
        return count;
    }
}