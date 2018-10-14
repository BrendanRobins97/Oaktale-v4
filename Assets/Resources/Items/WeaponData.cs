using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon", order = 1)]
public class WeaponData : ItemData {

    public GameObject weapon;

    private Weapon wep;

    public override void Use1(PlayerInfo playerInfo)
    {
        wep.Attack();
    }

    public override void Use2(PlayerInfo playerInfo)
    {
        wep.SpecialAttack();
    }

    public override void Activate(PlayerInfo playerInfo)
    {                                                                                                           
        Instantiate(weapon, playerInfo.equip.transform);
        wep = playerInfo.equip.GetComponentInChildren<Weapon>();
        if (playerInfo.player.currentItem.id != 0) {
            WeaponItem wepItem = (WeaponItem)playerInfo.player.currentItem;
            wep.experience = wepItem.experience;

        }
    }

}
