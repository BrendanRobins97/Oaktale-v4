using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetInAttackRange : StateMachineBehaviour
{
    [SerializeField] private float targetDistance = 8f;
    private Transform target;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        target = FindObjectOfType<PlayerInfo>().transform;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if ((target.position - animator.transform.position).magnitude < targetDistance)
        {
            animator.SetBool("InAttackRange", true);
        }
    }
}