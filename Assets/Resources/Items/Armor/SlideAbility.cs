using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/SlideAbility")]
public class SlideAbility : Ability
{
    [SerializeField] private float slideSpeed;

    public override void UseAbility(PlayerInfo playerInfo)
    {
        Slide(playerInfo);
    }

    public void Slide(PlayerInfo playerInfo)
    {
        playerInfo.player.SetInvincible(1f);
        playerInfo.player.SetSlide(0.5f);
        playerInfo.player.SetAnimation("Slide", 0.5f);
        if (playerInfo.player.transform.lossyScale.x > 0)
        {
            playerInfo.player.velocity.x = slideSpeed;
        }
        else
        {
            playerInfo.player.velocity.x = -slideSpeed;
        }

    }
}
