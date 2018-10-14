// File: Character.cs
// Author: Brendan Robinson
// Date Created: 01/04/2018
// Date Last Modified: 07/25/2018
// Description: 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(Animator))]
public abstract class Character : MonoBehaviour
{
    public new string name;
    public float moveSpeed = 16;
    public float width;
    public float height;
    public float maxFallSpeed = 100;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .05f;
    public float jumpHeight = 8f;
    public float timeToJumpApex = .4f;
    [HideInInspector] public Vector2 velocity;
    [HideInInspector] public Animator animator;
    [HideInInspector] public bool facingRight = true;
    [HideInInspector] public bool jump = false;
    [HideInInspector] public Vector2 targetVelocity;
    public List<Int2> colPositions = new List<Int2>();
    protected int invincibleCount = 0;
    protected float velocityXSmoothing;
    protected Controller2D controller;
    protected float gravity;
    protected bool affectedByGravity = true;
    protected float jumpVelocity;
    protected Int2 blockPosition = Int2.one;
    protected Int2 oldBlockPosition = Int2.zero;

    public bool Invicible
    {
        get { return invincibleCount > 0; }
    }

    protected virtual void Start()
    {
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();

        width = controller.collider.size.x;
        height = controller.collider.size.y;

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    protected virtual void Update()
    {
    }

    public virtual void AddForce(Vector2 force)
    {
        velocity += force;
    }

    public virtual void HitCharacter(Vector2 velocity)
    {
        this.velocity = velocity;
        controller.collisions.below = false;
    }

    public void SetInvincible(float invincibleDuration)
    {
        StartCoroutine(Invincible(invincibleDuration));
    }

    private IEnumerator Invincible(float invincibleDuration)
    {
        invincibleCount++;
        yield return new WaitForSeconds(invincibleDuration);
        invincibleCount--;
    }

    public void SetAnimation(string parameter, float duration)
    {
        StartCoroutine(Animation(parameter, duration));
    }


    private IEnumerator Animation(string parameter, float duration)
    {
        animator.SetBool(parameter, true);
        yield return new WaitForSeconds(duration);
        animator.SetBool(parameter, false);
    }
    public void SetSlide(float duration)
    {
        StartCoroutine(Slide(duration));
    }
    private IEnumerator Slide(float duration)
    {
        BoxCollider2D playerCollider = GetComponent<BoxCollider2D>();
        Vector2 initialSize = playerCollider.size;
        Vector2 initialOffset = playerCollider.offset;
        playerCollider.size = new Vector2(initialSize.x, initialSize.y / 2);
        playerCollider.offset = new Vector2(initialOffset.x, initialOffset.y / 2);

        yield return new WaitForSeconds(duration);
        playerCollider.size = initialSize;
        playerCollider.offset = initialOffset;

    }
}