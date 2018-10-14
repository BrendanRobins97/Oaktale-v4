using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(Animator))]
public class Enemy : Character
{
    public int experienceToGive = 10;

    public State currentState;
    public State remainState;

    [HideInInspector] public Transform target;
    [HideInInspector] public float stateTimeElapsed;
    private bool aiActive = true;
    private bool switchedState = true;

    protected override void Start() {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update() {
        if (!aiActive)
            return;
        if (switchedState) {
            currentState.OnEnterState(this);
            switchedState = false;
        }
        currentState.UpdateState(this);
        HandleMovement();
        animator.SetFloat("Speed", Mathf.Abs(velocity.x));
    }

    public void HandleMovement() {
        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }

        if (targetVelocity.x > 0 && transform.lossyScale.x < 0) {
            facingRight = true;
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (targetVelocity.x < 0 && transform.lossyScale.x > 0) {
            facingRight = false;
            transform.localScale = new Vector3(-1, 1, 1);
        }

        float smoothTime;

        if (Mathf.Abs(velocity.x) > moveSpeed || !controller.collisions.below) {
            smoothTime = accelerationTimeAirborne;
        } else {
            smoothTime = accelerationTimeGrounded;
        }
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity.x, ref velocityXSmoothing, smoothTime);

        velocity.y += gravity * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y, -maxFallSpeed, float.MaxValue);
        controller.Move(velocity * Time.deltaTime);
    }

    public void TransitionToState(State nextState)
    {
        if (nextState != remainState)
        {
            currentState = nextState;
            OnExitState();
        }
    }

    public bool CheckIfCountDownElapsed(float duration)
    {
        stateTimeElapsed += Time.deltaTime;
        return (stateTimeElapsed > duration);
    }

    private void OnExitState()
    {
        stateTimeElapsed = 0;
        switchedState = true;
    }

    private void OnDestroy() {
        GameManager.Get<PlayerInfo>().player.experience += experienceToGive;
    }
}