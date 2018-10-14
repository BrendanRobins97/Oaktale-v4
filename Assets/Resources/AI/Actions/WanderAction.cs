using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/WanderAction")]
public class WanderAction : Action
{
    [SerializeField] private float wanderMoveSpeed = 2f;
    [SerializeField] private float wanderSmoothness = 1f;
    [Range(0, 0.5f)] [SerializeField] private float wanderAmount = 0.25f;


    public override void Act(Enemy enemy)
    {
        float randomWanderValue = Utilities.PerlinNoise(Time.timeSinceLevelLoad, enemy.gameObject.GetInstanceID(), wanderSmoothness, 1, 1);
        if (randomWanderValue < wanderAmount)
        {
            enemy.targetVelocity.x = -wanderMoveSpeed;
        }
        else if (randomWanderValue > 1 - wanderAmount)
        {
            enemy.targetVelocity.x = wanderMoveSpeed;
        }
        else
        {
            enemy.targetVelocity.x = 0;
        }
    }
}