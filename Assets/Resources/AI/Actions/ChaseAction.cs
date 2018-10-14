using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/ChaseAction")]
public class ChaseAction : Action
{
    [SerializeField] private float chaseMoveSpeed = 6f;
    [SerializeField] private float stopDistance = 0.5f;

    public override void Act(Enemy enemy)
    {
        if (enemy.target == null) return;

        float targetDistance = enemy.target.position.x - enemy.transform.position.x;
        if (targetDistance < -stopDistance)
        {
            enemy.targetVelocity.x = -chaseMoveSpeed;
        }
        else if (targetDistance > stopDistance)
        {
            enemy.targetVelocity.x = chaseMoveSpeed;
        }
        else
        {
            enemy.targetVelocity.x = 0;
        }
    }
}