// File: Tool.cs
// Author: Brendan Robinson
// Date Created: 01/06/2018
// Date Last Modified: 07/14/2018
// Description: 

using System.Collections;
using UnityEngine;

public class Tool : MonoBehaviour
{
    private Animator animator;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
    }

    public virtual void PrimaryUse()
    {
        animator.SetTrigger("Attack");
        StartCoroutine(AttackTimer());
    }

    public virtual void SecondaryUse()
    {
    }

    private IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(0.1f);
        animator.ResetTrigger("Attack");
    }
}