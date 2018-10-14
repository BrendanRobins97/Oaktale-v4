using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/DashAbility")]
public class DashAbility : Ability {

    [SerializeField] private float dashSpeed;

	public override void UseAbility(PlayerInfo playerInfo)
    {
        Dash(playerInfo);
    }

    public void Dash(PlayerInfo playerInfo)
    {
        playerInfo.player.SetInvincible(1f);
        if (playerInfo.player.transform.lossyScale.x > 0)
        {
            playerInfo.player.velocity.x = dashSpeed;
        } else
        {
            playerInfo.player.velocity.x = -dashSpeed;
        }

    }
}
