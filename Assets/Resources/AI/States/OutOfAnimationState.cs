using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/AnimationStateDecision")]
public class AnimationState : Decision {

    [SerializeField] private string animation;

    public override bool Decide(Enemy enemy)
    {
        return enemy.animator.GetBool(animation);
    }
}
