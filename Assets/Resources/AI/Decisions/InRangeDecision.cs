using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/InRangeDecision")]
public class InRangeDecision : Decision
{
    [SerializeField] private float range = 20f;

    public override bool Decide(Enemy controller)
    {
        return (controller.transform.position - controller.target.transform.position).magnitude <= range;
    }

}