using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponItem : Item {

    public int experience;
    public int level;

    public WeaponItem(int id = 0, int experience = 0) {
        this.id = id;
        this.experience = experience;
    }
}
