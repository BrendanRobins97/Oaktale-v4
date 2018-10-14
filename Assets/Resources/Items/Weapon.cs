using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public Animator animator;
    public int experience;

    public void Attack()
    {
        animator.SetTrigger("Attack");
        StartCoroutine(AttackTimer());
    }
    public void SpecialAttack()
    {

    }
    IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(0.1f);
        animator.ResetTrigger("Attack");
    }
}
