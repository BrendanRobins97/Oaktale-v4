using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Mouse/ThrowWeapon")]
public class MouseThrowWeaponAction : Action
{
    
    [SerializeField] private float velocity = 15f;
    [SerializeField] private float gravity = 30f;
    [SerializeField] private float pauseAfterThrowDuration = 1.5f;

    private MouseAxe thrownAxe;
    private bool hasAxe = true;

    public override void InitialAction(Enemy enemy)
    {
        enemy.targetVelocity.x = 0;
        thrownAxe = enemy.GetComponentInChildren<MouseAxe>();
        if (thrownAxe!=null)
        {
            thrownAxe.StartCoroutine(thrownAxe.Throw(velocity, gravity, pauseAfterThrowDuration));

        }
    }

}
