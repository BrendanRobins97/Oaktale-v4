using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Modifier { Bigger, Quicker, Aggressive, Haste }

[RequireComponent(typeof(BoxCollider2D))]
public class SlimeAI : Character {

    private bool idle, angry, bite;
    private GameObject player;
    private CapsuleCollider2D aggroCollider;
    private float angerTime = 1f;
    private float resetAggro = 1.5f;
    public Modifier modifier;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
        modifier = (Modifier)Random.Range((int)0, 3);
        collisionMask = LayerMask.GetMask("Tiles");
        ApplyModifier();
        horizontalRayCount = (int)(4 * width);
        verticalRayCount = (int)(4 * height);
        SetJumpValues(jumpHeight, timeToJumpApex);
        CalculateRaySpacing();

    }
    protected void ApplyModifier()
    {
        switch (modifier)
        {
            case Modifier.Bigger:
                transform.localScale *= 2;
                width *= 2;
                height *= 2;
                break;
            case Modifier.Quicker:
                moveSpeed *= 2;
                break;
            case Modifier.Aggressive:
                GetComponentInChildren<CapsuleCollider2D>().size *= 1.5f;
                break;
            case Modifier.Haste:
                resetAggro /= 3;
                break;
            default:
                break;
        }
    }
    protected override void Update()
    {
        HandleMovement();
        if (angry) animator.SetTrigger("Angry");
        else animator.ResetTrigger("Angry");
        if (bite) animator.SetTrigger("Bite");
        else animator.ResetTrigger("Bite");
        if (idle) animator.SetTrigger("Idle");
        else animator.ResetTrigger("Idle");
        if (hit)
        {
            StopAllCoroutines();
            idle = true;
            angry = false;
            bite = false;
        }
        hit = false;
    }

    IEnumerator Angry()
    {
        idle = false;
        angry = true;

        yield return new WaitForSeconds(angerTime);
        angry = false;
        bite = true;
        Vector2 dir = (player.transform.position - this.transform.position).normalized * moveSpeed;
        dir.y = Mathf.Clamp(dir.y, 0, float.MaxValue);
        if (dir.x > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        }
        HitCharacter(dir + Vector2.up*20);
        collisions.below = false;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        yield return new WaitForSeconds(resetAggro);
        bite = false;
        idle = true;
    }

    

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!angry && !bite && collision is BoxCollider2D && collision.gameObject.tag.Equals("Player"))
        {
            player = collision.gameObject;
            StartCoroutine(Angry());
        }
    }
    protected override void HandleMovement()
    {
        if (collisions.above || collisions.below)
        {
            velocity.y = 0;
        }
        if (collisions.left || collisions.right)
        {
            velocity.x = 0;
        }
        float targetVelocityX = 0;
        if (targetVelocityX != 0)
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (collisions.below) ? accelerationTimeGrounded * 3 : accelerationTimeAirborne);
        }
        else
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        }

        velocity.y += gravity * Time.deltaTime;
        Move(velocity * Time.deltaTime);
    }

    protected override void Move(Vector2 velocity)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;
        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);
    }

}
