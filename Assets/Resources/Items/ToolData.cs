using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Tool", menuName = "Tool", order = 1)]
public class ToolData : ItemData
{
    public GameObject tool;

    public override void Use1(PlayerInfo playerInfo)
    {
        playerInfo.equip.GetComponentInChildren<Tool>().PrimaryUse();
    }

    public override void Use2(PlayerInfo playerInfo)
    {
        playerInfo.equip.GetComponentInChildren<Tool>().SecondaryUse();
    }

    public override void Activate(PlayerInfo playerInfo)
    {
        Instantiate(tool, playerInfo.equip.transform);
    }

}