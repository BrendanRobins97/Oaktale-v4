using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Mouse/RetrieveWeaponAction")]
public class MouseRetrieveWeaponAction : Action {

    [SerializeField] private float retrieveMoveSpeed = 10f;

    public override void InitialAction(Enemy controller)
    {
        controller.target = controller.GetComponent<MoleWeapon>().weapon;
    }
    public override void Act(Enemy enemy)
    {
        if (enemy.target == null) return;

        float targetDistance = enemy.target.position.x - enemy.transform.position.x;
        if (targetDistance < 0)
        {
            enemy.targetVelocity.x = -retrieveMoveSpeed;
        }
        else if (targetDistance > 0)
        {
            enemy.targetVelocity.x = retrieveMoveSpeed;
        }
        else
        {
            enemy.targetVelocity.x = 0;
        }
    }
}
