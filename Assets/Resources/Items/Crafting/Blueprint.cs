using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Blueprint", menuName = "Blueprint", order = 1)]
public class Blueprint : ScriptableObject
{
    public ItemData[] inputs;
    public int[] inputAmounts;

    public ItemData output;
    public int outputAmount;
}

