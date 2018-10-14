// File: MouseAxe.cs
// Author: Brendan Robinson
// Date Created: 06/23/2018
// Date Last Modified: 07/30/2018
// Description: 

using System.Collections;
using UnityEngine;

public class MouseAxe : MonoBehaviour
{
    [SerializeField] private State retreiveWeaponState;
    [SerializeField] private State wanderState;
    [SerializeField] private float rotationSpeed = 1080f;
    [SerializeField] private AnimationClip throwAnimation;

    private Vector3 velocity;
    private float gravity;
    private bool frozen = true;
    private DamageSource damageSource;
    private Enemy enemy;
    private Vector2 initialPosition;

    private float destroyAfterDeathTime = 1f;

    // Use this for initialization
    private void Start()
    {
        damageSource = GetComponent<DamageSource>();
        enemy = GetComponentInParent<Enemy>();
        initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!frozen)
        {
            damageSource.enabled = true;
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime * transform.localScale.x);
            velocity.y -= gravity * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
        }
        else
        {
            damageSource.enabled = false;
        }

        if (enemy == null) Destroy(gameObject, destroyAfterDeathTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Collider")) frozen = true;
        if (frozen && enemy != null && collision.gameObject.gameObject.GetInstanceID()
                .Equals(enemy.gameObject.GetInstanceID()))
        {
            transform.SetParent(enemy.transform);
            transform.localPosition = initialPosition;
            transform.rotation = new Quaternion(0, 0, 0, 0);
            transform.localScale = new Vector3(-1, 1, 1);
            enemy.TransitionToState(wanderState);
        }
    }

    public IEnumerator Throw(float axeVelocity, float axeGravity, float pauseDuration)
    {
        Vector2 playerCoordinates = enemy.target.position - enemy.transform.position;
        Vector2 normalPlayerCoordinates = playerCoordinates.normalized;

        enemy.animator.SetBool("Attack", true);
        yield return new WaitForSeconds(throwAnimation.length);
        enabled = true;
        Vector3 currentPosition = enemy.transform.position;
        transform.SetParent(enemy.transform.parent);
        transform.position = enemy.transform.position + Vector3.up * 2;
        transform.rotation = Quaternion.identity;
        frozen = false;
        velocity.x = normalPlayerCoordinates.x * axeVelocity;
        velocity.y = (normalPlayerCoordinates.y + 0.25f + Random.Range(0f, 0.75f)) * axeVelocity;
        gravity = axeGravity;

        transform.localScale = new Vector3(Mathf.Sign(playerCoordinates.x), 1, 1);

        yield return new WaitForSeconds(pauseDuration);
        if (enemy)
        {
            enemy.animator.SetBool("Attack", false);
            enemy.TransitionToState(retreiveWeaponState);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}