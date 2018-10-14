using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationManager : MonoBehaviour
{

    public Dictionary<Int2, GameObject> decorations = new Dictionary<Int2, GameObject>();

    public void AddDecoration(Int2 pos, GameObject decorationObj)
    {
        decorations.Add(pos, decorationObj);
    }

    public void DeleteDecoration(Int2 pos)
    {
        GameObject decorationObj = decorations[pos];
        decorations.Remove(pos);
        Destroy(decorationObj);
    }
}
