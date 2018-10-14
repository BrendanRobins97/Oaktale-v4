using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ArmorType { Helmet, Chest, Legs, Belt, Boots, Gloves, Necklace, Ring}

[CreateAssetMenu(fileName = "Armor", menuName = "Amor", order = 1)]
public class ArmorData : ItemData {
    public ArmorType armorType;

    public Ability ability;
}
