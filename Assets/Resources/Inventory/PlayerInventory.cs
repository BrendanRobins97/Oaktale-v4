using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInventory : Inventory
{

    public static readonly int helmetIndex = 40;
    public static readonly int glovesIndex = 41;
    public static readonly int chestIndex = 42;
    public static readonly int legsIndex = 43;
    public static readonly int bootsIndex = 44;
    public static readonly int necklaceIndex = 45;
    public static readonly int ring1Index = 46;
    public static readonly int ring2Index = 47;
    public static readonly int beltIndex = 48;
    public static readonly int weaponIndex = 49;

    public Item GetHelmet()
    {
        return items[helmetIndex];
    }
    public Item GetGloves()
    {
        return items[glovesIndex];
    }
    public Item GetChest()
    {
        return items[chestIndex];
    }
    public Item GetLegs()
    {
        return items[legsIndex];
    }
    public Item GetBoots()
    {
        return items[bootsIndex];
    }
    public Item GetNecklace()
    {
        return items[necklaceIndex];
    }
    public Item GetRing1()
    {
        return items[ring1Index];
    }
    public Item GetRing2()
    {
        return items[ring2Index];
    }
    public Item GetBelt()
    {
        return items[beltIndex];
    }
    public WeaponItem GetWeapon()
    {
        return (WeaponItem)items[weaponIndex];
    }

    protected override void Awake()
    {
        base.Awake();
        playerInfo.uiHandler.playerInventory = inventoryPanel.gameObject;
        OnInventoryChanged += playerInfo.actionBar.UpdateActionBarImages;
    }

    protected override void SetSlotUI()
    {
        Slot[] transforms = inventoryPanel.GetComponentsInChildren<Slot>();
        for (int i = 0; i < transforms.Length; i++)
        {
            items.Add(new Item());
            slots.Add(transforms[i]);
            slots[i].transform.SetParent(inventoryPanel.transform);
            slots[i].slotNumber = i;
            slots[i].inventory = this;
        }
        inventoryPanel.transform.SetAsFirstSibling();
        inventoryPanel.SetActive(false);
        InventoryChanged();
    }

    public override void AddItem(Item item, int amount = 1, int slot = 0)
    {
        base.AddItem(item, amount, slot);
        playerInfo.actionBar.UpdateActionBarImages();
    }

    public override void RemoveItem(Item item, int amount = 1, int slot = 0)
    {
        base.RemoveItem(item, amount);
        playerInfo.actionBar.UpdateActionBarImages();
    }

}
