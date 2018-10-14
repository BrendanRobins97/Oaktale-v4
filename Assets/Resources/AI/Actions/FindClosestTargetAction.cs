using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/FindClosestTargetAction")]
public class FindClosestTargetAction : Action
{
    public override void InitialAction(Enemy controller)
    {
        controller.target = FindObjectOfType<PlayerInfo>().transform;
    }
}